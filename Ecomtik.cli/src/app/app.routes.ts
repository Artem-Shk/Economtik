import { Routes } from '@angular/router';
import { IntegralCalculatorComponent } from './integral-calculator/integral-calculator.component';

export const routes: Routes = [
  {
    path: '',
    component: IntegralCalculatorComponent,
    pathMatch: 'full'
  }
];
