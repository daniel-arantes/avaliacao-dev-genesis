import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CalculateCdbRequest, CdbCalculation } from './investment.models';

@Injectable({ providedIn: 'root' })
export class InvestmentApiService {
  private readonly http = inject(HttpClient);
  calculate(request: CalculateCdbRequest): Observable<CdbCalculation> {
    return this.http.post<CdbCalculation>('/api/investments/cdb/calculate', request);
  }
}
