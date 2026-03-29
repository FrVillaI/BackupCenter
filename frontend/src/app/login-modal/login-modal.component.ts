import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

/**
 * Modal de login reutilizable.
 *
 * Responsabilidades:
 * - Solicitar credenciales al usuario
 * - Autenticarse contra el backend
 * - Emitir eventos al componente padre
 *
 * Este componente NO maneja almacenamiento de token,
 * solo lo devuelve al padre (patrón desacoplado).
 */
@Component({
  selector: 'app-login-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.css']
})
export class LoginModalComponent {

  /** Usuario ingresado */
  username = '';

  /** Contraseña ingresada */
  password = '';

  /** Mensaje de error para UI */
  error = '';

  /**
   * Evento emitido cuando el login es exitoso
   * Retorna el token JWT
   */
  @Output() success = new EventEmitter<string>();

  /**
   * Evento emitido cuando el usuario cancela el modal
   */
  @Output() cancel = new EventEmitter<void>();

  constructor(private http: HttpClient) {}

  /**
   * Ejecuta autenticación contra el backend.
   *
   * Flujo:
   * 1. Envía credenciales a /api/auth/login
   * 2. Si es exitoso → emite token al padre
   * 3. Si falla → muestra mensaje de error
   */
  login() {
    this.http.post<{ token: string }>('/api/auth/login', {
      username: this.username,
      password: this.password
    }).subscribe({
      next: res => {
        /**
         * Se emite el token, pero NO se guarda aquí.
         * El componente padre decide qué hacer con él.
         */
        this.success.emit(res.token);
      },
      error: () => {
        this.error = 'Usuario o contraseña incorrecta';
      }
    });
  }
}