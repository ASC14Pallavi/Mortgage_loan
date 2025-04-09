
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule,FormGroup, Validators, FormBuilder, ReactiveFormsModule, FormControl } from '@angular/forms';
import { LoanService } from '../core/services/loan.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { InterestRateService } from '../core/services/interest-rate.service';


@Component({
  selector: 'app-loan-application',
  standalone: true,
  templateUrl: './loan-application.component.html',
  styleUrls: ['./loan-application.component.css'],
  imports: [CommonModule, FormsModule,ReactiveFormsModule]
})
export class LoanApplicationComponent implements OnInit {
  loanForm !: FormGroup;
  interestRates: any[] = [];
  constructor(private loanService: LoanService, public router: Router,private fb: FormBuilder,
    private toastr: ToastrService,private interestRateService: InterestRateService) {
    this.loanForm = this.fb.group({
      loanAmount: ['', [Validators.required, Validators.min(1000)]],
      loanTermYears: ['', [Validators.required, Validators.min(1), Validators.max(30)]],
      interestRate: new FormControl({ value: '', disabled: true }),
      interestRateId: ['', Validators.required] // Only store the ID
    });
  }

  ngOnInit(): void {
    this.interestRateService.getSelectedRate().subscribe((rateInfo) => {
      if (rateInfo) {
        this.loanForm.patchValue({
          interestRate: rateInfo.rate,
          interestRateId: rateInfo.rateId
        });
      }
    });
  }

  submitLoan() {
    if (this.loanForm.invalid) {
      this.toastr.error('Please fill in all required fields.');
      return;
    }

    const loanData = {
      loanAmount: this.loanForm.value.loanAmount,
      loanTermYears: this.loanForm.value.loanTermYears,
      interestRateId: this.loanForm.value.interestRateId // Send only ID
    };

    this.loanService.applyForLoan(loanData).subscribe(
      (response) => {
        this.toastr.success('Loan application submitted successfully!');
        this.loanForm.reset();
      },
      (error) => {
        this.toastr.error('Failed to submit loan application.');
      }
    );
  }
}