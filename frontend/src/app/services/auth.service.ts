import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

interface JwtResponse { token: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {

  private tokenKey = 'token';

  // CAMBIO CLAVE: URL ABSOLUTA A LA API
  private apiUrl = 'http://localhost:5000/api/auth'; 


  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string): Observable<JwtResponse> {
    return this.http.post<JwtResponse>(`${this.apiUrl}/login`, { username, password }).pipe(
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