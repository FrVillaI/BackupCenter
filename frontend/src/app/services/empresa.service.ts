import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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

@Injectable({ providedIn: 'root' })
export class EmpresaService {
  constructor(private http: HttpClient) { }
  getEmpresas(): Observable<Empresa[]> {
    return this.http.get<Empresa[]>('/api/empresas', {
    });
  }

  toggleActiva(id: number) {
    return this.http.put(`/api/empresas/${id}/toggle-activa`, {});
  }

}