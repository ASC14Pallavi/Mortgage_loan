import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { authGuard } from './core/guards/auth.guard';
import { InterestRateComponent } from './components/interest-rate/interest-rate.component';
import { LoanApplicationComponent } from './loan-application/loan-application.component';
import { AmortizationScheduleComponent } from './amortization-schedule/amortization-schedule.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
    { path: 'amortization-schedule', component: AmortizationScheduleComponent,canActivate: [authGuard] }, 
    { path: 'interest-rates', component: InterestRateComponent ,canActivate: [authGuard]},
    { path: 'loan-application', component: LoanApplicationComponent ,canActivate: [authGuard]},
    // { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
    { path: '', redirectTo: '/login', pathMatch: 'full' },

   
];
