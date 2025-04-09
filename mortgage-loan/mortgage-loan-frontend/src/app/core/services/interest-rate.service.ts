import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class InterestRateService {
  private selectedRate = new BehaviorSubject<{ rate: number, rateId: number } | null>(null);
  selectedRate$ = this.selectedRate.asObservable();
  private apiUrl = 'https://localhost:5001/api/Loan/interest-rates'; // Adjust API URL

  constructor(private http: HttpClient) {}

  getInterestRates() {
    return this.http.get<any[]>(this.apiUrl);
  }

  setSelectedRate(rate: number, rateId: number) {
    this.selectedRate.next({ rate, rateId });
  }

  getSelectedRate() {
    return this.selectedRate.asObservable();
  }
}
