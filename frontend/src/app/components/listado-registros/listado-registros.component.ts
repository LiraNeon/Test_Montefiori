import { Component, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Registro } from '../../models/registro';

@Component({
  selector: 'app-listado-registros',
  imports: [FormsModule],
  templateUrl: './listado-registros.component.html',
  styleUrl: './listado-registros.component.css'
})
export class ListadoRegistrosComponent {
  registros = input.required<Registro[]>();
  eliminando = input<number | null>(null);
  editandoId = input<number | null>(null);

  eliminar = output<number>();
  editar = output<number>();
  cancelar = output<void>();
  guardar = output<Registro>();

  // Copia del registro que se está editando, para no modificar la fila original hasta guardar
  borrador: Registro | null = null;

  empezarEdicion(r: Registro): void {
    this.borrador = { ...r };
    this.editar.emit(r.id);
  }

  confirmarEliminar(r: Registro): void {
    if (confirm(`¿Eliminar el registro de ${r.nombre}?`)) {
      this.eliminar.emit(r.id);
    }
  }
}
