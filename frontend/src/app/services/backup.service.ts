import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BackupService {
  constructor(private http: HttpClient) {}

  backup(empresaId: number, overridePath?: string): Observable<any> {
    const url = `/api/backup/${empresaId}?overridePath=${overridePath ?? ''}`;
    return this.http.post(url, {});
  }

  listBackups(): Observable<any> {
    return this.http.get('/api/backups');
  }
}