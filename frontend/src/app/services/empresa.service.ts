import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Empresa {
  id: number;
  nombre: string;
  rutaOrigen: string;
  activa: boolean;
  frecuencia: string;
  horaProgramada: string;
  ultimaCopia?: string;
}

@Injectable({ providedIn: 'root' })
export class EmpresaService {
  constructor(private http: HttpClient) {}
  getEmpresas(): Observable<Empresa[]> {
    return this.http.get<Empresa[]>('/api/empresas');
  }
}