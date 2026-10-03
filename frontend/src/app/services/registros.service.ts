import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Registro, ResultadoCarga } from '../models/registro';

@Injectable({ providedIn: 'root' })
export class RegistrosService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/registros`;

  obtenerTodos(): Observable<Registro[]> {
    return this.http.get<Registro[]>(this.url);
  }

  subirArchivo(archivo: File): Observable<ResultadoCarga> {
    const form = new FormData();
    form.append('archivo', archivo); // el nombre debe coincidir con el parámetro del controlador
    return this.http.post<ResultadoCarga>(`${this.url}/upload`, form);
  }

  actualizar(registro: Registro): Observable<Registro> {
    return this.http.put<Registro>(`${this.url}/${registro.id}`, registro);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}

/** Convierte un error HTTP en un mensaje legible para el usuario. */
export function mensajeDeError(err: HttpErrorResponse): string {
  if (err.status === 0) return 'No se pudo conectar con el servidor.';
  return err.error?.mensaje ?? `Error inesperado (${err.status}).`;
}
