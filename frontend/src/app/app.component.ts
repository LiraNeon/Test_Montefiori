import { Component, OnInit, inject, signal } from '@angular/core';
import { CargaArchivoComponent } from './components/carga-archivo/carga-archivo.component';
import { ListadoRegistrosComponent } from './components/listado-registros/listado-registros.component';
import { Registro } from './models/registro';
import { RegistrosService, mensajeDeError } from './services/registros.service';

@Component({
  selector: 'app-root',
  imports: [CargaArchivoComponent, ListadoRegistrosComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private service = inject(RegistrosService);

  registros = signal<Registro[]>([]);
  cargando = signal(false);
  eliminando = signal<number | null>(null);
  editandoId = signal<number | null>(null);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.cargarRegistros();
  }

  cargarRegistros(): void {
    this.cargando.set(true);
    this.service.obtenerTodos().subscribe({
      next: (data) => {
        this.registros.set(data);
        this.error.set(null);
        this.cargando.set(false);
      },
      error: (err) => {
        this.error.set(mensajeDeError(err));
        this.cargando.set(false);
      }
    });
  }

  guardar(registro: Registro): void {
    this.service.actualizar(registro).subscribe({
      next: (actualizado) => {
        this.registros.update((lista) => lista.map((r) => (r.id === actualizado.id ? actualizado : r)));
        this.error.set(null);
        this.editandoId.set(null);
      },
      error: (err) => this.error.set(mensajeDeError(err))
    });
  }

  eliminar(id: number): void {
    this.eliminando.set(id);
    this.service.eliminar(id).subscribe({
      next: () => {
        this.registros.update((lista) => lista.filter((r) => r.id !== id));
        this.error.set(null);
        this.eliminando.set(null);
      },
      error: (err) => {
        this.error.set(mensajeDeError(err));
        this.eliminando.set(null);
      }
    });
  }
}
