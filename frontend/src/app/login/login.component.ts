import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Componente de login de la aplicación.
 *
 * Responsabilidades:
 * - Capturar credenciales del usuario
 * - Invocar el servicio de autenticación
 * - Redirigir al dashboard en caso de éxito
 * - Manejar errores básicos de autenticación
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule], 
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  /** Usuario ingresado en el formulario */
  username = '';

  /** Contraseña ingresada */
  password = '';

  /**
   * Controla visibilidad del campo password
   * - 'password' → oculto
   * - 'text' → visible
   */
  passwordFieldType: 'password' | 'text' = 'password';

  /**
   * Rol del usuario autenticado (placeholder actual)
   */
  userRole?: string;

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  /**
   * Ejecuta el proceso de login.
   *
   * Flujo:
   * 1. Valida campos básicos
   * 2. Llama a AuthService.login()
   * 3. Guarda token (internamente en el servicio)
   * 4. Redirige al dashboard
   */
  login(form: any) {

    // Validación mínima en frontend
    if (!this.username || !this.password) return;

    this.auth.login(this.username, this.password).subscribe({
      next: () => {

        /**
         * Hardcode temporal
         * Ideal: obtener rol desde el JWT o backend
         */
        this.userRole = 'user';

        // Redirección tras login exitoso
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error('Login failed', err);

        // Mensaje básico de error
        alert('Credenciales incorrectas');
      }
    });
  }

  /**
   * Alterna visibilidad del campo password
   */
  togglePasswordVisibility() {
    this.passwordFieldType =
      this.passwordFieldType === 'password' ? 'text' : 'password';
  }
}