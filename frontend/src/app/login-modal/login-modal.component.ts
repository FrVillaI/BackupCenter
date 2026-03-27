import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.css']
})

export class LoginModalComponent {
  username = '';
  password = '';
  error = '';

  @Output() success = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<void>();

  constructor(private http: HttpClient) { }

  login() {
    this.http.post<{ token: string }>('/api/auth/login', {
      username: this.username,
      password: this.password
    }).subscribe({
      next: res => this.success.emit(res.token),
      error: err => this.error = 'Usuario o contraseña incorrecta'
    });
  }
}
