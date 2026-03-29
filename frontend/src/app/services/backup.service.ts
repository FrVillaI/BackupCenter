import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Representa el resultado de un respaldo generado por el backend.
 *
 * Campos:
 * - zip: nombre o ruta del archivo comprimido generado
 * - hash: hash de integridad del respaldo
 * - hashPath: ruta donde se almacena el archivo de hash
 */
export interface BackupResult {
  zip: string;
  hash: string;
  hashPath: string;
}

/**
 * Servicio encargado de interactuar con el microservicio de respaldos.
 *
 * Responsabilidades:
 * - Ejecutar respaldos manuales
 * - Consultar historial de respaldos
 *
 * Este servicio actúa como capa de abstracción entre:
 * UI (componentes) ↔ API REST (backend .NET)
 */
@Injectable({ providedIn: 'root' })
export class BackupService {

  /**
   * URL base del endpoint de respaldos
   * - Respeta mayúsculas/minúsculas según backend (.NET es case-sensitive en rutas)
   * - Debería moverse a environment.ts en producción
   */
  private apiUrl = 'http://localhost:5000/api/Backups';

  constructor(private http: HttpClient) { }

  /**
   * Ejecuta un respaldo para una empresa específica.
   *
   * Endpoint:
   * POST /api/Backups/{empresaId}
   *
   * Flujo:
   * 1. Construye parámetros opcionales (query string)
   * 2. Envía request POST vacío (body {})
   * 3. Recibe metadata del respaldo generado
   */
  backup(empresaId: number, overridePath?: string): Observable<BackupResult> {

    /**
     * HttpParams es inmutable, por lo que cada set()
     * devuelve una nueva instancia
     */
    let params = new HttpParams();

    // Se agrega overridePath solo si el usuario lo proporciona
    if (overridePath) {
      params = params.set('overridePath', overridePath);
    }

    return this.http.post<BackupResult>(
      `${this.apiUrl}/${empresaId}`,
      {}, // Body vacío: el backend usa solo route + query params
      { params }
    );
  }

  /**
   * Obtiene el listado de respaldos generados.
   * Endpoint:
   * GET /api/Backups
   */
  listBackups(): Observable<BackupResult[]> {
    return this.http.get<BackupResult[]>(`${this.apiUrl}`);
  }
}