import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:5001/api/auth';
  private tokenKey = 'jwtToken';
  public loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  private http = inject(HttpClient);
  private router = inject(Router);
  private toastr = inject(ToastrService);

  login(username: string, password: string): Observable<boolean> {
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, { username, password })
      .pipe(
        map(response => {
          if (response.token) {
            localStorage.setItem(this.tokenKey, response.token);
            this.loggedIn.next(true);
            this.toastr.success('Login successful!', 'Welcome');
            return true;
          }
          return false;
        }),
        catchError(error => {
          this.toastr.error('Invalid credentials. Please try again.', 'Login Failed');
          throw error;
        })
      );
  }

  isAuthenticated(): boolean {
        return this.loggedIn.value;
      }
    
  getToken(): string | null {
      return localStorage.getItem(this.tokenKey);
    }
    

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.loggedIn.next(false);
    this.toastr.info('You have been logged out.', 'Logout');
    this.router.navigate(['/login']);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
  
  register(username: string, email: string, password: string): Observable<any> {
    return this.http.post('https://localhost:5001/api/Auth/register', {
      username,
      email,
      password
    });
  }
  
}
