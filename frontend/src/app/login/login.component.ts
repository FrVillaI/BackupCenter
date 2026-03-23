import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule], // ✅ Angular 18 requiere imports
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username = '';
  password = '';
  passwordFieldType: 'password' | 'text' = 'password';
  userRole?: string;

  constructor(private auth: AuthService, private router: Router) {}

  login(form: any) {
    if (!this.username || !this.password) return;

    this.auth.login(this.username, this.password).subscribe({
      next: (res) => {
        this.userRole = 'user'; 
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error('Login failed', err);
        alert('Credenciales incorrectas');
      }
    });
  }

  togglePasswordVisibility() {
    this.passwordFieldType = this.passwordFieldType === 'password' ? 'text' : 'password';
  }
}