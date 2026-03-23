import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Empresa, EmpresaService } from '../services/empresa.service';
import { BackupService } from '../services/backup.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {
  empresas: Empresa[] = [];
  selectedIds: Set<number> = new Set<number>();

  constructor(private es: EmpresaService, private bs: BackupService) {}

  ngOnInit(): void {
    this.es.getEmpresas().subscribe(data => this.empresas = data);
  }

  toggleSelection(id: number, event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.checked) this.selectedIds.add(id);
    else this.selectedIds.delete(id);
  }

  startBackupSelected() {
    this.selectedIds.forEach(id => {
      this.bs.backup(id).subscribe({
        next: res => console.log(`Backup realizado para empresa ${id}`, res),
        error: err => console.error(`Error al respaldar empresa ${id}`, err)
      });
    });
  }
}