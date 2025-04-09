import { Component, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Chart, registerables } from 'chart.js/auto';
import { Loan,AmortizationSchedule } from '../models/loan';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, AfterViewInit {
  userLoans: Loan[] = [];
  selectedLoanId: number | null = null;
  amortizationSchedule: AmortizationSchedule[] = [];
  totalPrincipal: number = 0;
  totalInterest: number = 0;
  loanAmount: number = 0;
  loanTerm: number = 0;
  interestRate: number = 0;

  combinedChart: Chart | null = null;

  constructor(private http: HttpClient) {
    Chart.register(...registerables);
  }

  ngOnInit() {
    this.fetchUserLoans();
  }

  ngAfterViewInit() {
    setTimeout(() => this.renderChart(), 100);
  }

  fetchUserLoans() {
    this.http.get<Loan[]>('https://localhost:5001/api/Loan/user-loans')
      .subscribe({
        next: (response) => {
          this.userLoans = response;
          if (this.userLoans.length > 0) {
            this.selectedLoanId = this.userLoans[0].id;
            this.fetchAmortizationSchedule();
          }
        },
        error: (err) => console.error('Error fetching user loans:', err)
      });
  }

  fetchAmortizationSchedule() {
    if (!this.selectedLoanId) return;
    this.http.get<AmortizationSchedule[]>(`https://localhost:5001/api/Loan/dashboard/amortization-data/${this.selectedLoanId}`)
      .subscribe({
        next: (response) => {
          this.amortizationSchedule = response;
          this.totalPrincipal = this.amortizationSchedule.reduce((sum, a) => sum + a.principalPayment, 0);
          this.totalInterest = this.amortizationSchedule.reduce((sum, a) => sum + a.interestPayment, 0);
          this.updateLoanDetails();
          this.renderChart();
        },
        error: (err) => console.error('Error fetching amortization schedule:', err)
      });
  }

  updateLoanDetails() {
    const selectedLoan = this.userLoans.find(loan => loan.id === this.selectedLoanId);
    if (selectedLoan) {
      this.loanAmount = selectedLoan.loanAmount;
      this.loanTerm = selectedLoan.loanTermYears;
      this.interestRate = selectedLoan.interestRate;
    }
  }

  renderChart() {
    const ctx = document.getElementById('combinedChart') as HTMLCanvasElement;
    if (!ctx) return;
    if (this.combinedChart) this.combinedChart.destroy();
  
    this.combinedChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: this.amortizationSchedule.map(a => `#${a.paymentNumber}`),
        datasets: [
          {
            type: 'line',
            label: 'Remaining Balance',
            data: this.amortizationSchedule.map(a => a.remainingBalance),
            borderColor: '#FF5733', // Vivid orange-red
            backgroundColor: 'rgba(255, 87, 51, 0.2)',
            fill: false,
            yAxisID: 'y',
          },
          {
            label: 'Principal Payment',
            data: this.amortizationSchedule.map(a => a.principalPayment),
            backgroundColor: '#33B5E5', // Bright sky blue
            yAxisID: 'y1',
          },
          {
            label: 'Interest Payment',
            data: this.amortizationSchedule.map(a => a.interestPayment),
            backgroundColor: '#9C27B0', // Deep purple
            yAxisID: 'y1',
          },
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            labels: {
              color: '#333',
              font: {
                size: 14,
                weight: 'bold'
              }
            }
          },
          tooltip: {
            backgroundColor: '#f4f4f4',
            titleColor: '#000',
            bodyColor: '#000',
            borderColor: '#ccc',
            borderWidth: 1
          }
        },
        scales: {
          y: {
            type: 'linear',
            position: 'left',
            title: {
              display: true,
              text: 'Remaining Balance',
              color: '#FF5733',
              font: {
                weight: 'bold'
              }
            },
            ticks: {
              color: '#FF5733'
            }
          },
          y1: {
            type: 'linear',
            position: 'right',
            title: {
              display: true,
              text: 'Payments',
              color: '#33B5E5',
              font: {
                weight: 'bold'
              }
            },
            grid: {
              drawOnChartArea: false
            },
            ticks: {
              color: '#33B5E5'
            }
          }
        }
      }
    });
  }
  
}  