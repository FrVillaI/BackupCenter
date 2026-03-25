import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Empresa, EmpresaService } from '../services/empresa.service';
import { BackupService } from '../services/backup.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  empresas: Empresa[] = [];
  selectedIds: Set<number> = new Set<number>();
  loadingIds: Set<number> = new Set<number>();
  importPath: string = '';
  expandedId: number | null = null;   // ← nuevo: controla qué card está abierta

  isBulkLoading: boolean = false;

  bulkTotal: number = 0;
  bulkProgress: number = 0;

  toastMessage: string = '';

  constructor(
    private es: EmpresaService,
    private bs: BackupService,
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.loadEmpresas();
  }

  loadEmpresas(): void {
    this.es.getEmpresas().subscribe(data => this.empresas = data);
  }

  // ── Expand / collapse card ─────────────────────────────────
  toggleExpand(id: number): void {
    this.expandedId = this.expandedId === id ? null : id;
  }

  // ── Checkbox ───────────────────────────────────────────────
  toggleSelection(id: number, event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.checked) this.selectedIds.add(id);
    else this.selectedIds.delete(id);
  }

  // ── Format date ────────────────────────────────────────────
  formatDate(dateStr?: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleString();
  }

  // ── Single backup ──────────────────────────────────────────
  backupSingle(id: number): void {
    this.loadingIds.add(id);

    this.bs.backup(id).subscribe({
      next: res => {
        console.log(`Backup realizado para empresa ${id}`, res);
        this.updateUltimaCopia(id);
        this.showToast(`Backup completado (Empresa ${id})`);
      },
      error: err => {
        console.error(`Error al respaldar empresa ${id}`, err);
        this.showToast(`Error en backup (Empresa ${id})`);
      },
      complete: () => this.loadingIds.delete(id)
    });
  }

  // ── Bulk backup ────────────────────────────────────────────
  startBackupSelected(): void {
    if (this.selectedIds.size === 0) return;

    this.isBulkLoading = true;

    const ids = Array.from(this.selectedIds);

    this.bulkTotal = ids.length;
    this.bulkProgress = 0;

    ids.forEach(id => {
      this.bs.backup(id).subscribe({
        next: () => { },
        error: () => { },
        complete: () => {
          this.bulkProgress++;

          if (this.bulkProgress === this.bulkTotal) {
            this.isBulkLoading = false;
            this.showToast(`Backup masivo completado (${this.bulkTotal} empresas)`);
          }
        }
      });
    });
  }

  // ── Update last backup timestamp locally ──────────────────
  updateUltimaCopia(id: number): void {
    const empresa = this.empresas.find(e => e.id === id);
    if (empresa) empresa.ultimaCopia = new Date().toISOString();
  }

  // ── Import DBF ─────────────────────────────────────────────
  importDbf(): void {
    if (!this.importPath) return alert('Debe ingresar una ruta válida');

    this.http.post(`/api/ImportDbf?path=${encodeURIComponent(this.importPath)}`, {})
      .subscribe({
        next: res => {
          console.log('ImportDbf realizado', res);
          alert('Importación exitosa');
          this.loadEmpresas();
        },
        error: err => {
          console.error('Error al importar DBF', err);
          alert('Error al importar DBF');
        }
      });
  }
  
  showToast(message: string): void {
    this.toastMessage = message;

    setTimeout(() => {
      this.toastMessage = '';
    }, 3000);
  }
}

