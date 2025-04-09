import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AmortizationScheduleResponse } from '../../models/loan';

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private apiUrl = 'https://localhost:5001/api/Loan'; 
  private toastr = inject(ToastrService);


  constructor(private http: HttpClient) {}

  getLoans(): Observable<any[]> {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<any[]>(this.apiUrl, { headers });
  }
   // Submit a loan application
   applyForLoan(loanData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}`, loanData).pipe(
      catchError(error => this.handleError(error, 'Failed to apply for a loan'))
    );
  }

  // Fetch interest rates from backend
  getInterestRates(): Observable<any> {
    return this.http.get(`https://localhost:5001/api/Loan/interest-rates`).pipe(
      catchError(error => this.handleError(error, 'Failed to fetch interest rates'))
    );
  }

  // Get loan details by ID
  getLoanDetails(loanId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${loanId}`).pipe(
      catchError(error => this.handleError(error, 'Failed to fetch loan details'))
    );
  }

  // Fetch amortization schedule for a loan
  getAmortizationSchedule(loanId: number, page: number, size: number): Observable<AmortizationScheduleResponse> {
    return this.http.get<AmortizationScheduleResponse>(`${this.apiUrl}/${loanId}/schedule?page=${page}&size=${size}`)
   ;
}
// Fetch amortization schedule for a loan (corrected API endpoint)
getAmortization(loanId: number): Observable<AmortizationScheduleResponse> {
  return this.http.get<AmortizationScheduleResponse>(`${this.apiUrl}/dashboard/amortization-data/${loanId}`).pipe(
    catchError(error => this.handleError(error, 'Failed to fetch amortization schedule'))
  );
}

  getUserLoans(): Observable<any[]> {
    return this.http.get<any[]>('https://localhost:5001/api/Loan/user-loans'); // Adjust API endpoint
  }

  // Handle errors and display Toastr notifications
  private handleError(error: HttpErrorResponse, message: string) {
    console.error('API Error:', error);
    this.toastr.error(message, 'Error');
    return throwError(() => new Error(message));
  }
}
