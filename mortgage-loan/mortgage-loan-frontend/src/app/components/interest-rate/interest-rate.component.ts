import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { InterestRateService } from '../../core/services/interest-rate.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-interest-rate',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './interest-rate.component.html',
  styleUrl: './interest-rate.component.css'
})
export class InterestRateComponent implements OnInit {
  interestRates: any[] = [];

  constructor(private interestRateService: InterestRateService, private router: Router) {}

  ngOnInit(): void {
    this.interestRateService.getInterestRates().subscribe((rates) => {
      this.interestRates = rates;
    });
  }

  selectRate(rate: number, rateId: number): void {
    this.interestRateService.setSelectedRate(rate, rateId);
    this.router.navigate(['/loan-application']); // Navigate to Loan Application
  }
}
