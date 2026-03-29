import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

/**
 * Estructura esperada de la respuesta del backend
 * al momento de autenticarse.
 */
interface JwtResponse {
  token: string;
}

/**
 * Servicio de autenticación encargado de:
 * - Gestionar login/logout
 * - Almacenar el token JWT
 * - Proveer utilidades de sesión
 *
 * Este servicio es utilizado por:
 * - Interceptores (para adjuntar token)
 * - Guards (para proteger rutas)
 * - Componentes de login
 */
@Injectable({ providedIn: 'root' })
export class AuthService {

  /**
   * Clave utilizada para almacenar el token en localStorage
   */
  private tokenKey = 'token';

  /**
   * URL base del endpoint de autenticación
   */
  private apiUrl = 'http://localhost:5000/api/auth';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  /**
   * Realiza la autenticación contra el backend.
   *
   * Flujo:
   * 1. Envía credenciales al endpoint /login
   * 2. Recibe un JWT
   * 3. Guarda el token en localStorage
   */
  login(username: string, password: string): Observable<JwtResponse> {
    return this.http.post<JwtResponse>(
      `${this.apiUrl}/login`,
      { username, password }
    ).pipe(
      tap(res => {
        /**
         * tap() permite ejecutar efectos secundarios
         * sin modificar el flujo del observable
         */
        if (res?.token) {
          localStorage.setItem(this.tokenKey, res.token);
        }
      })
    );
  }

  /**
   * Cierra la sesión del usuario.
   *
   * Acciones:
   * - Elimina el token almacenado
   * - Redirige al login
   */
  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }

  /**
   * Obtiene el token almacenado (si existe)
   */
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  /**
   * Verifica si el usuario está autenticado.
   * Esta validación SOLO comprueba existencia del token,
   * no su validez o expiración.
   */
  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}