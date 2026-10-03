import { Component, ElementRef, inject, output, signal, viewChild } from '@angular/core';
import { RegistrosService, mensajeDeError } from '../../services/registros.service';

@Component({
  selector: 'app-carga-archivo',
  templateUrl: './carga-archivo.component.html',
  styleUrl: './carga-archivo.component.css'
})
export class CargaArchivoComponent {
  private service = inject(RegistrosService);

  /** Avisa al padre que se cargaron registros nuevos. */
  cargado = output<number>();

  private input = viewChild.required<ElementRef<HTMLInputElement>>('inputArchivo');

  archivo = signal<File | null>(null);
  cargando = signal(false);
  error = signal<string | null>(null);
  exito = signal<string | null>(null);

  seleccionar(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0] ?? null;
    this.exito.set(null);
    this.error.set(null);

    if (file && !file.name.toLowerCase().endsWith('.xlsx')) {
      this.error.set('Solo se permiten archivos .xlsx.');
      this.limpiar();
      return;
    }
    this.archivo.set(file);
  }

  subir(): void {
    const file = this.archivo();
    if (!file) return;

    this.cargando.set(true);
    this.error.set(null);
    this.exito.set(null);

    this.service.subirArchivo(file).subscribe({
      next: (res) => {
        this.exito.set(`Se cargaron ${res.insertados} registros.`);
        this.cargando.set(false);
        this.limpiar();
        this.cargado.emit(res.insertados);
      },
      error: (err) => {
        this.error.set(mensajeDeError(err));
        this.cargando.set(false);
      }
    });
  }

  private limpiar(): void {
    this.archivo.set(null);
    this.input().nativeElement.value = '';
  }
}
