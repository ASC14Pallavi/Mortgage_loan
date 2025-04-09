export interface AmortizationScheduleEntry {
    paymentNumber: number;
    paymentDate: string;
    principalPayment: number;
    interestPayment: number;
    remainingBalance: number;
  }
  
  export interface AmortizationScheduleResponse {
    totalRecords: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    data: AmortizationScheduleEntry[];
  }

  export interface Loan {
    id: number;
    loanAmount: number;
    loanTermYears: number;
    interestRate: number;
  }
  
  export interface AmortizationSchedule {
    paymentNumber: number;
    paymentDate: string;
    principalPayment: number;
    interestPayment: number;
    remainingBalance: number;
  }