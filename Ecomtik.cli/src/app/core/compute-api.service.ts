import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ComputeRequestBody,
  ComputeResponseBody,
  HistoryEntry,
  IntegrationMethod
} from './models/compute.models';
import { API_BASE_URL } from './tokens/api-base-url.token';

@Injectable({ providedIn: 'root' })
export class ComputeApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  private computeRoot(): string {
    const b = this.baseUrl.replace(/\/$/, '');
    return b ? `${b}/api/Compute` : '/api/Compute';
  }

  getMethods(): Observable<IntegrationMethod[]> {
    return this.http.get<IntegrationMethod[]>(`${this.computeRoot()}/methods`);
  }

  getHistory(limit = 10): Observable<HistoryEntry[]> {
    const params = new HttpParams().set('limit', String(limit));
    return this.http.get<HistoryEntry[]>(`${this.computeRoot()}/history`, { params });
  }

  compute(body: ComputeRequestBody): Observable<ComputeResponseBody> {
    return this.http.post<ComputeResponseBody>(this.computeRoot(), body);
  }
}
