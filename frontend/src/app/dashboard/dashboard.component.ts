import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Empresa, EmpresaService } from '../services/empresa.service';
import { BackupService } from '../services/backup.service';
import { HttpClient } from '@angular/common/http';
import { LoginModalComponent } from '../login-modal/login-modal.component';

/**
 * Dashboard principal del sistema de respaldos.
 *
 * Responsabilidades:
 * - Mostrar empresas activas/inactivas
 * - Ejecutar backups individuales y masivos
 * - Gestionar selección múltiple
 * - Mostrar progreso de operaciones
 * - Importar configuraciones desde DBF
 * - Controlar autenticación para acciones críticas
 *
 * Este componente actúa como orquestador entre:
 * UI ↔ Servicios (EmpresaService, BackupService)
 */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, LoginModalComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  /** Controla visibilidad del modal de login */
  showLoginModalFlag = false;

  /** ID pendiente cuando se requiere autenticación */
  backupPendingId: number | null = null;

  /** Token temporal (flujo manual) */
  authToken: string = '';

  /** Lista completa de empresas */
  empresas: Empresa[] = [];

  /** Subconjuntos para UI */
  empresasActivas: Empresa[] = [];
  empresasInactivas: Empresa[] = [];

  /** IDs seleccionados para backup masivo */
  selectedIds: Set<number> = new Set<number>();

  /** IDs actualmente en proceso (loading individual) */
  loadingIds: Set<number> = new Set<number>();

  /** Ruta para importación DBF */
  importPath: string = '';

  /** Controla expansión de tarjetas en UI */
  expandedId: number | null = null;

  /** Estado de carga masiva */
  isBulkLoading: boolean = false;

  /** Progreso de backup masivo */
  bulkTotal: number = 0;
  bulkProgress: number = 0;

  /** Mensaje de notificación */
  toastMessage: string = '';

  constructor(
    private es: EmpresaService,
    private bs: BackupService,
    private http: HttpClient
  ) {}

  /** Inicialización del componente */
  ngOnInit(): void {
    this.loadEmpresas();
  }

  /**
   * Carga empresas desde backend y separa por estado
   */
  loadEmpresas(): void {
    this.es.getEmpresas().subscribe(data => {
      this.empresas = data;

      this.empresasActivas = data.filter(e => e.activa);
      this.empresasInactivas = data.filter(e => !e.activa);
    });
  }

  // ─────────────────────────────────────────────
  // UI STATE
  // ─────────────────────────────────────────────

  /** Expande o colapsa una tarjeta */
  toggleExpand(id: number): void {
    this.expandedId = this.expandedId === id ? null : id;
  }

  /** Manejo de selección múltiple */
  toggleSelection(id: number, event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.checked) this.selectedIds.add(id);
    else this.selectedIds.delete(id);
  }

  /** Formatea fechas para UI */
  formatDate(dateStr?: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleString();
  }

  // ─────────────────────────────────────────────
  // BACKUP INDIVIDUAL
  // ─────────────────────────────────────────────

  /**
   * Ejecuta backup para una sola empresa
   */
  backupSingle(id: number): void {
    const empresa = this.empresas.find(e => e.id === id);

    // Validación: no permitir respaldo en empresas inactivas
    if (!empresa?.activa) {
      this.showToast('Empresa inactiva no puede hacer backup');
      return;
    }

    this.loadingIds.add(id);

    this.bs.backup(id).subscribe({
      next: res => {
        console.log(`Backup realizado para empresa ${id}`, res);

        // Se guarda metadata del respaldo en memoria
        empresa.lastBackup = {
          zip: res.zip,
          hash: res.hash,
          hashPath: res.hashPath
        };

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

  // ─────────────────────────────────────────────
  // BACKUP MASIVO
  // ─────────────────────────────────────────────

  /**
   * Ejecuta backups en paralelo para empresas seleccionadas
   */
  startBackupSelected(): void {
    if (this.selectedIds.size === 0) return;

    this.isBulkLoading = true;

    // Solo empresas activas
    const ids = Array.from(this.selectedIds).filter(id => {
      const e = this.empresas.find(emp => emp.id === id);
      return e?.activa;
    });

    this.bulkTotal = ids.length;
    this.bulkProgress = 0;

    ids.forEach(id => {
      this.bs.backup(id).subscribe({
        next: () => { },
        error: () => { },
        complete: () => {
          this.bulkProgress++;

          // Finalización del proceso masivo
          if (this.bulkProgress === this.bulkTotal) {
            this.isBulkLoading = false;
            this.showToast(`Backup masivo completado (${this.bulkTotal} empresas)`);
          }
        }
      });
    });
  }

  // ─────────────────────────────────────────────
  // UTILIDADES
  // ─────────────────────────────────────────────

  /** Actualiza fecha de último respaldo localmente */
  updateUltimaCopia(id: number): void {
    const empresa = this.empresas.find(e => e.id === id);
    if (empresa) empresa.ultimaCopia = new Date().toISOString();
  }

  /** Muestra notificación temporal */
  showToast(message: string): void {
    this.toastMessage = message;

    setTimeout(() => {
      this.toastMessage = '';
    }, 3000);
  }

  // ─────────────────────────────────────────────
  // IMPORTACIÓN
  // ─────────────────────────────────────────────

  /**
   * Importa configuración desde archivo DBF
   */
  importDbf(): void {
    if (!this.importPath) {
      alert('Debe ingresar una ruta válida');
      return;
    }

    this.http.post(`/api/ImportDbf?path=${encodeURIComponent(this.importPath)}`, {})
      .subscribe({
        next: () => {
          alert('Importación exitosa');
          this.loadEmpresas();
        },
        error: () => {
          alert('Error al importar DBF');
        }
      });
  }

  // ─────────────────────────────────────────────
  // CONFIGURACIÓN EMPRESA
  // ─────────────────────────────────────────────

  /** Alterna estado activo/inactivo */
  toggleActiva(id: number): void {
    this.es.toggleActiva(id).subscribe({
      next: () => {
        this.showToast('Estado actualizado');
        this.loadEmpresas();
      },
      error: () => this.showToast('Error al cambiar estado')
    });
  }

  /** Guarda configuración de frecuencia/hora */
  guardarConfiguracion(e: Empresa): void {
    this.http.put(`/api/empresas/${e.id}/config`, {
      frecuenciaHoras: e.frecuenciaHoras,
      horaProgramada: e.horaProgramada
    }).subscribe({
      next: () => {
        this.showToast(`Configuración guardada (${e.nombre})`);
      },
      error: () => {
        this.showToast(`Error guardando configuración`);
      }
    });
  }

  // ─────────────────────────────────────────────
  // AUTENTICACIÓN MANUAL
  // ─────────────────────────────────────────────

  /**
   * Ejecuta backup requiriendo login previo
   */
  backupManualWithAuth(id: number) {
    if (!this.authToken) {
      this.backupPendingId = id;
      this.showLoginModalFlag = true;
      return;
    }

    this.backupSingle(id);
  }

  /** Callback al login exitoso */
  onLoginSuccess(token: string) {
    this.authToken = token;
    this.showLoginModalFlag = false;

    if (this.backupPendingId !== null) {
      this.backupSingle(this.backupPendingId);
      this.backupPendingId = null;
    }
  }

  /** Cancelación de login */
  onLoginCancel() {
    this.showLoginModalFlag = false;
    this.backupPendingId = null;
  }
}