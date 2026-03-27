import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BackupResult {
  zip: string;
  hash: string;
  hashPath: string;
}

@Injectable({ providedIn: 'root' })
export class BackupService {
  private apiUrl = 'http://localhost:5000/api/Backups'; // 🔹 B mayúscula

  constructor(private http: HttpClient) {}

  // Respaldo de empresa con ruta opcional
  backup(empresaId: number, overridePath?: string): Observable<BackupResult> {
    let params = new HttpParams();
    if (overridePath) {
      params = params.set('overridePath', overridePath);
    }
    return this.http.post<BackupResult>(`${this.apiUrl}/${empresaId}`, {}, { params });
  }

  // Listado de respaldos
  listBackups(): Observable<BackupResult[]> {
    return this.http.get<BackupResult[]>(`${this.apiUrl}`);
  }
}