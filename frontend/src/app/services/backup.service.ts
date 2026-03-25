import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BackupService {
  private apiUrl = 'http://localhost:5000/api/Backups'; // 🔹 B mayúscula como en tu API

  constructor(private http: HttpClient) {}

  // Respaldo de empresa con ruta opcional
  backup(empresaId: number, overridePath?: string): Observable<any> {
    let params = new HttpParams();
    if (overridePath) {
      params = params.set('overridePath', overridePath);
    }
    return this.http.post(`${this.apiUrl}/${empresaId}`, {}, { params });
  }

  // Listado de respaldos
  listBackups(): Observable<any> {
    return this.http.get(`${this.apiUrl}`);
  }
}