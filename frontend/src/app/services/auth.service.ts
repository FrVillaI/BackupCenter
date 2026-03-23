import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

interface JwtResponse { token: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'token';
  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string): Observable<JwtResponse> {
    return this.http.post<JwtResponse>('/api/auth/login', { username, password }).pipe(
      tap(res => {
        if (res?.token) {
          localStorage.setItem(this.tokenKey, res.token);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
