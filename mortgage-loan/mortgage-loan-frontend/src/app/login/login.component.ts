import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [FormsModule, CommonModule]
})
export class LoginComponent {
  username = '';
  password = '';
  email = ''; // <-- for registration
  isRegistering = false;
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router, private toastr: ToastrService) {}

  login() {
    this.authService.login(this.username, this.password).subscribe({
      next: (success) => {
        if (success) {
          this.router.navigate(['/dashboard']);
        }
      },
      error: () => {
        this.errorMessage = 'Invalid credentials';
      }
    });
  }

  register() {
    this.authService.register(this.username, this.email, this.password).subscribe({
      next: () => {
        this.toastr.success('Registration successful! Please log in.');
        this.isRegistering = false; // Switch to login view
        this.clearForm();
      },
      error: (err) => {
        console.log('Registration Error:', err);
      
        let message = 'Registration failed';
      
        // Try to extract meaningful error messages
        if (err?.error) {
          if (typeof err.error === 'string') {
            message = err.error;
          } else if (err.error.errors) {
            // For validation errors like { errors: { Password: ["..."], Email: ["..."] } }
            const errorsArray = Object.values(err.error.errors).flat();
            message = errorsArray.join(' ');
          } else if (err.error.title) {
            message = err.error.title;
          }
        }
        console.log('Error Response:', err.error);

        this.toastr.error(message);
      }
      
    });
  }

  toggleRegister() {
    this.isRegistering = !this.isRegistering;
    this.clearForm();
  }

  clearForm() {
    this.username = '';
    this.password = '';
    this.email = '';
    this.errorMessage = '';
  }
  
}
