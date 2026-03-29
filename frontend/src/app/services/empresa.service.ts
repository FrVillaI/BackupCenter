import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Modelo de una empresa dentro del sistema de respaldos.
 * Representa la configuración necesaria para ejecutar backups
 * automáticos o manuales.
 */
export interface Empresa {
  id: number;
  nombre: string;
  rutaOrigen: string;
  activa: boolean;
  frecuenciaHoras: number;
  horaProgramada: string;
  ultimaCopia?: string;

  lastBackup?: {
    zip: string;
    hash: string;
    hashPath: string;
  };
}

/**
 * Servicio encargado de gestionar operaciones relacionadas
 * con las empresas configuradas en el sistema.
 *
 * Responsabilidades:
 * - Obtener listado de empresas
 * - Activar / desactivar respaldos por empresa
 */
@Injectable({ providedIn: 'root' })
export class EmpresaService {

  constructor(private http: HttpClient) { }

  /**
   * Obtiene todas las empresas registradas.
   * Endpoint:
   * GET /api/empresas
   * Uso típico:
   * - Dashboard principal
   * - Configuración de respaldos
   */
  getEmpresas(): Observable<Empresa[]> {
    return this.http.get<Empresa[]>('/api/empresas');
  }

  /**
   * Alterna el estado "activa" de una empresa.
   * Endpoint:
   * PUT /api/empresas/{id}/toggle-activa
   * Comportamiento:
   * - Si está activa → la desactiva
   * - Si está inactiva → la activa
   */
  toggleActiva(id: number) {
    return this.http.put(`/api/empresas/${id}/toggle-activa`, {});
  }

}