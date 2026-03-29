import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

/**
 * Interceptor HTTP encargado de adjuntar automáticamente
 * el token JWT en todas las solicitudes salientes.
 *
 * Este interceptor se ejecuta antes de que cualquier request
 * salga de la aplicación Angular hacia el backend.
 *
 * Flujo:
 * 1. Obtiene el token desde AuthService
 * 2. Si existe, clona la request original
 * 3. Agrega el header Authorization: Bearer <token>
 * 4. Continúa la cadena de interceptores
 *
 * Beneficio:
 * - Centraliza la autenticación
 * - Evita repetir lógica en cada servicio HTTP
 */

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  /**
   * Servicio encargado de manejar autenticación
   * (almacenamiento y recuperación del token)
   */
  constructor(private auth: AuthService) {}

  /**
   * Método principal del interceptor.
   *
   * Request HTTP original (inmutable)
   * Handler que permite continuar con la cadena
   * Observable del evento HTTP
   */
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    
    // Obtener el token actual (si el usuario está autenticado)
    const token = this.auth.getToken();
    if (token) {
      const cloned = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      return next.handle(cloned);
    }
    return next.handle(req);
  }
}