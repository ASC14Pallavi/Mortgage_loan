import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Chart } from 'chart.js/auto';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Loan ,AmortizationSchedule} from '../models/loan';


@Component({
  selector: 'app-amortization-schedule',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './amortization-schedule.component.html',
  styleUrls: ['./amortization-schedule.component.css']
})
export class AmortizationScheduleComponent implements OnInit {
  userLoans: Loan[] = [];
  selectedLoanId: number | null = null;
  amortizationSchedule: AmortizationSchedule[] = [];
  pageNumber: number = 1;
  pageSize: number = 10;
  totalPages: number = 1;
  amortizationChart: any;
  scheduleExists: boolean = false;
  isLoading: boolean = false;  // Spinner state

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchUserLoans();
  }

  fetchUserLoans() {
    this.http.get<Loan[]>('https://localhost:5001/api/Loan/user-loans')
      .subscribe(response => {
        this.userLoans = response;
        if (this.userLoans.length > 0) {
          this.selectedLoanId = this.userLoans[0].id;
          this.fetchAmortizationSchedule();
        }
      });
  }

  fetchAmortizationSchedule() {
    if (!this.selectedLoanId) return;
    this.isLoading = true;
  
    this.http.get<any>(`https://localhost:5001/api/Loan/${this.selectedLoanId}/amortization-schedule?pageNumber=${this.pageNumber}&pageSize=${this.pageSize}`)
      .subscribe(response => {
        console.log("Raw API Response:", response);  // Debugging
        if (response?.data?.length) {
          // Remove duplicates
          this.amortizationSchedule = this.removeDuplicates(response.data);
          this.totalPages = response.totalPages;
          this.scheduleExists = true;
  
          setTimeout(() => this.renderChart(), 200);
        } else {
          this.scheduleExists = false;
        }this.isLoading = false;
      }, error => {
        console.error('Error fetching schedule:', error);
        this.scheduleExists = false;
        this.isLoading = false;
      });
  }
  removeDuplicates(schedule: AmortizationSchedule[]): AmortizationSchedule[] {
    const seen = new Set();
    return schedule.filter(item => {
      const key = `${item.paymentNumber}-${item.paymentDate}-${item.principalPayment}-${item.interestPayment}-${item.remainingBalance}`;
      if (seen.has(key)) return false;
      seen.add(key);
      return true;
    });
  }
  
  

  generateAmortizationSchedule() {
    if (!this.selectedLoanId) return;
    this.isLoading = true;
    this.http.post<AmortizationSchedule[]>(`https://localhost:5001/api/Loan/${this.selectedLoanId}/generate-amortization-schedule`, {})
      .subscribe({
        next: (schedule) => {
          console.log('Generated and saved schedule:', schedule);
          this.amortizationSchedule = schedule;
          this.scheduleExists = true;
          this.renderChart();
          this.isLoading = false;
        },
        error: (err) =>  {
          console.error('Error generating schedule:', err);
          this.isLoading = false;
        }
        
      });
  }

  prevPage() {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.fetchAmortizationSchedule();
    }
  }

  nextPage() {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.fetchAmortizationSchedule();
    }
  }

  renderChart() {
    if (this.amortizationChart) {
      this.amortizationChart.destroy();
    }
  
    const ctx = document.getElementById('amortizationChart') as HTMLCanvasElement;
    if (!ctx) return;
  
    const labels = this.amortizationSchedule.map(a => `${a.paymentNumber}`);
    const principalData = this.amortizationSchedule.map(a => a.principalPayment);
    const interestData = this.amortizationSchedule.map(a => a.interestPayment);
  
    this.amortizationChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: labels,
        datasets: [
          {
            label: 'Principal Payment',
            data: principalData,
            backgroundColor: 'rgba(40, 167, 69, 0.8)',    // Emerald Green
            borderColor: 'rgba(40, 167, 69, 1)',
            borderWidth: 1,
          },
          {
            label: 'Interest Payment',
            data: interestData,
            backgroundColor: 'rgba(255, 159, 64, 0.8)',   // Amber Orange
            borderColor: 'rgba(255, 159, 64, 1)',
            borderWidth: 1,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
      },
    });
  }
}  