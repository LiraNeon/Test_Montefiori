export interface Registro {
  id: number;
  nombre: string;
  email: string;
  telefono: string | null;
  distrito: string | null;
  sexo: string | null;
}

export interface ResultadoCarga {
  insertados: number;
}
