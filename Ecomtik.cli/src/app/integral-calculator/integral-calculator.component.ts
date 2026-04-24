import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  afterNextRender,
  Component,
  computed,
  inject,
  signal
} from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ComputeApiService } from '../core/compute-api.service';
import type {
  ComputeResponseBody,
  HistoryEntry,
  IntegrationMethod
} from '../core/models/compute.models';

const FALLBACK_METHODS: IntegrationMethod[] = [
  { id: -1, code: 'simpson', name: 'Симпсон' },
  { id: -2, code: 'trapezoidal', name: 'Метод трапеций' },
  { id: -3, code: 'monte_carlo', name: 'Монте-Карло' }
];

@Component({
  selector: 'app-integral-calculator',
  imports: [DatePipe, DecimalPipe],
  templateUrl: './integral-calculator.component.html',
  styleUrl: './integral-calculator.component.scss'
})
export class IntegralCalculatorComponent {
  private readonly api = inject(ComputeApiService);

  protected readonly functionExpr = signal('x*x');
  protected readonly boundA = signal('0');
  protected readonly boundB = signal('1');
  protected readonly steps = signal(1000);
  protected readonly methodCode = signal('simpson');

  protected readonly methods = signal<IntegrationMethod[]>(FALLBACK_METHODS);
  protected readonly methodsLoading = signal(false);
  protected readonly history = signal<HistoryEntry[]>([]);
  protected readonly historyLoading = signal(false);
  protected readonly submitting = signal(false);
  protected readonly lastResult = signal<ComputeResponseBody | null>(null);
  protected readonly error = signal<string | null>(null);

  protected readonly canSubmit = computed(() => {
    if (this.submitting()) {
      return false;
    }
    const fn = this.functionExpr().trim();
    if (!fn || !this.methodCode()) {
      return false;
    }
    const a = this.parseNumber(this.boundA());
    const b = this.parseNumber(this.boundB());
    const n = Math.trunc(this.steps());
    if (!Number.isFinite(a) || !Number.isFinite(b) || !Number.isFinite(n)) {
      return false;
    }
    return n >= 1;
  });

  constructor() {
    afterNextRender(() => {
      void this.refreshMethods();
      void this.refreshHistory();
    });
  }

  protected onFunctionInput(ev: Event): void {
    this.functionExpr.set((ev.target as HTMLInputElement).value);
  }

  protected onBoundAInput(ev: Event): void {
    this.boundA.set((ev.target as HTMLInputElement).value);
  }

  protected onBoundBInput(ev: Event): void {
    this.boundB.set((ev.target as HTMLInputElement).value);
  }

  protected onStepsInput(ev: Event): void {
    const raw = (ev.target as HTMLInputElement).value;
    this.steps.set(Number.parseInt(raw, 10) || 0);
  }

  protected onMethodChange(ev: Event): void {
    this.methodCode.set((ev.target as HTMLSelectElement).value);
  }

  protected async onSubmit(): Promise<void> {
    this.error.set(null);
    this.submitting.set(true);
    try {
      const a = this.parseNumber(this.boundA());
      const b = this.parseNumber(this.boundB());
      const body = {
        function: this.functionExpr().trim(),
        method: this.methodCode(),
        a,
        b,
        steps: Math.trunc(this.steps())
      };
      const res = await firstValueFrom(this.api.compute(body));
      this.lastResult.set(res);
      await this.refreshHistory();
    } catch (e) {
      this.lastResult.set(null);
      this.error.set(this.formatHttpError(e));
    } finally {
      this.submitting.set(false);
    }
  }

  private async refreshMethods(): Promise<void> {
    this.methodsLoading.set(true);
    try {
      const list = await firstValueFrom(this.api.getMethods());
      const next = list.length ? list : FALLBACK_METHODS;
      this.methods.set(next);
      const codes = new Set(next.map((m) => m.code));
      if (!codes.has(this.methodCode())) {
        this.methodCode.set(next[0]?.code ?? 'simpson');
      }
    } catch {
      this.methods.set(FALLBACK_METHODS);
      const fb = FALLBACK_METHODS;
      if (!fb.some((m) => m.code === this.methodCode())) {
        this.methodCode.set(fb[0]!.code);
      }
    } finally {
      this.methodsLoading.set(false);
    }
  }

  private async refreshHistory(): Promise<void> {
    this.historyLoading.set(true);
    try {
      const rows = await firstValueFrom(this.api.getHistory(10));
      this.history.set(rows);
    } catch {
      this.history.set([]);
    } finally {
      this.historyLoading.set(false);
    }
  }

  private parseNumber(raw: string): number {
    return Number.parseFloat(raw.replace(',', '.').trim());
  }

  private formatHttpError(err: unknown): string {
    if (err instanceof HttpErrorResponse) {
      if (typeof err.error === 'string' && err.error.trim()) {
        return err.error;
      }
      if (err.error && typeof err.error === 'object' && 'title' in err.error) {
        const t = (err.error as { title?: string }).title;
        if (t) {
          return t;
        }
      }
      return err.status
        ? `Ошибка сервера (${err.status}). Проверьте данные и соединение с API.`
        : 'Сеть недоступна. Убедитесь, что бэкенд запущен и прокси настроен.';
    }
    return 'Не удалось выполнить вычисление.';
  }
}
