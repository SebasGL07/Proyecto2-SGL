using System;

namespace GranjaSimulacion
{
    class Granja
    {
        public double Dinero { get; set; }
        public double CapitalInicial { get; private set; }
        public int NumEmpleados { get; set; }
        public double SueldoMensual { get; set; }
        public int MesesRestantes { get; set; }
        public int MesesSimulados { get; set; }
        public int Filas { get; set; }
        public int Columnas { get; set; }

        public Parcela[,] Parcelas { get; private set; }

        // indice 0=Trigo, 1=Repollo, 2=Tomate, 3=Calabaza, 4=Esparrago
        public int[] InventarioSemillas { get; private set; }

        public double TotalIngresos { get; set; }
        public double TotalMateriaPrima { get; set; }
        public double TotalManoObra { get; set; }

        // Tabla de cultivos
        private readonly string[] NombresCultivos = { "Trigo", "Repollo", "Tomate", "Calabaza", "Esparrago" };
        private readonly int[] MesesCultivos = { 1, 2, 3, 4, 6 };
        private readonly double[] CostosCultivos = { 100, 180, 250, 220, 500 };
        private readonly double[] IngresosCultivos = { 130, 280, 450, 360, 1000 };

        public Granja(double dinero, int numEmpleados, double sueldoMensual,
                      int meses, int filas, int columnas)
        {
            Dinero = dinero;
            CapitalInicial = dinero;
            NumEmpleados = numEmpleados;
            SueldoMensual = sueldoMensual;
            MesesRestantes = meses;
            MesesSimulados = 0;
            Filas = filas;
            Columnas = columnas;

            Parcelas = new Parcela[filas, columnas];
            for (int f = 0; f < filas; f++)
                for (int c = 0; c < columnas; c++)
                    Parcelas[f, c] = new Parcela();

            InventarioSemillas = new int[5];
            TotalIngresos = 0;
            TotalMateriaPrima = 0;
            TotalManoObra = 0;
        }

        public double CostosMensuales() => NumEmpleados * SueldoMensual;

        public double UtilidadProyectada() => Dinero - CostosMensuales();

        public void MostrarMapa()
        {
            Console.WriteLine("\n--- Mapa de Parcelas ---");
            Console.Write("        ");
            for (int c = 0; c < Columnas; c++)
                Console.Write($" Col{c + 1,-5}");
            Console.WriteLine();

            for (int f = 0; f < Filas; f++)
            {
                Console.Write($"  Fila{f + 1,-3} ");
                for (int c = 0; c < Columnas; c++)
                {
                    if (Parcelas[f, c].EstaOcupada)
                        Console.Write($" [{Parcelas[f, c].TipoSiembra.Substring(0, Math.Min(3, Parcelas[f, c].TipoSiembra.Length))}] ");
                    else
                        Console.Write(" [---] ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void ConsultarParcela(int fila, int columna)
        {
            Parcela p = Parcelas[fila, columna];
            Console.WriteLine($"\n--- Parcela ({fila + 1}, {columna + 1}) ---");
            Console.WriteLine($"  Estado        : {(p.EstaOcupada ? "Ocupada" : "Libre")}");

            if (p.EstaOcupada)
            {
                Console.WriteLine($"  Cultivo       : {p.TipoSiembra}");
                Console.WriteLine($"  Meses totales : {p.MesesParaCrecer}");
                Console.WriteLine($"  Meses restant.: {p.MesesRestantes}");
                Console.WriteLine($"  Ingreso esp.  : Q{p.IngresosCosecha:F2}");
            }
            else
            {
                Console.WriteLine($"  Ingresos esp. : Q0.00");
            }
        }

        public bool ComprarSemillas(int tipoIdx, int cantidad)
        {
            double costoTotal = CostosCultivos[tipoIdx] * cantidad;
            if (Dinero < costoTotal)
            {
                Console.WriteLine($"\n  [!] Dinero insuficiente. Necesita Q{costoTotal:F2}, tiene Q{Dinero:F2}.");
                return false;
            }
            Dinero -= costoTotal;
            TotalMateriaPrima += costoTotal;
            InventarioSemillas[tipoIdx] += cantidad;
            Console.WriteLine($"\n  Compra exitosa: {cantidad} semilla(s) de {NombresCultivos[tipoIdx]} por Q{costoTotal:F2}.");
            return true;
        }

        public bool Sembrar(int fila, int columna, int tipoIdx)
        {
            Parcela p = Parcelas[fila, columna];

            if (p.EstaOcupada)
            {
                Console.WriteLine("  [!] Esa parcela ya esta ocupada.");
                return false;
            }
            if (InventarioSemillas[tipoIdx] <= 0)
            {
                Console.WriteLine($"  [!] No tienes semillas de {NombresCultivos[tipoIdx]} en inventario.");
                return false;
            }

            InventarioSemillas[tipoIdx]--;
            p.TipoSiembra = NombresCultivos[tipoIdx];
            p.MesesParaCrecer = MesesCultivos[tipoIdx];
            p.MesesRestantes = MesesCultivos[tipoIdx];
            p.IngresosCosecha = IngresosCultivos[tipoIdx];
            p.EstaOcupada = true;

            Console.WriteLine($"\n  Sembrado {NombresCultivos[tipoIdx]} en parcela ({fila + 1}, {columna + 1}). Lista en {p.MesesRestantes} mes(es).");
            return true;
        }

        public bool AvanzarMes()
        {
            MesesSimulados++;
            MesesRestantes--;

            double salariosMes = CostosMensuales();
            Dinero -= salariosMes;
            TotalManoObra += salariosMes;

            Console.WriteLine($"\n  Mes {MesesSimulados}: Salarios pagados este mes: Q{salariosMes:F2}");

            if (Dinero <= 0)
            {
                Dinero = 0;
                Console.WriteLine("  [!] El dinero llego a Q0. Fin de la simulacion.");
                return false;
            }

            // Simular crecimiento y cosechar parcelas listas
            for (int f = 0; f < Filas; f++)
            {
                for (int c = 0; c < Columnas; c++)
                {
                    Parcela p = Parcelas[f, c];
                    if (!p.EstaOcupada) continue;

                    p.MesesRestantes--;

                    if (p.MesesRestantes <= 0)
                    {
                        Dinero += p.IngresosCosecha;
                        TotalIngresos += p.IngresosCosecha;
                        Console.WriteLine($"  Parcela ({f + 1},{c + 1}): {p.TipoSiembra} cosechado. +Q{p.IngresosCosecha:F2}");
                        p.Limpiar();
                    }
                    else
                    {
                        Console.WriteLine($"  Parcela ({f + 1},{c + 1}): {p.TipoSiembra} creciendo. Meses restantes: {p.MesesRestantes}");
                    }
                }
            }

            Console.WriteLine($"\n  Caja actual: Q{Dinero:F2} | Meses restantes: {MesesRestantes}");

            if (MesesRestantes <= 0)
            {
                Console.WriteLine("  Se completaron todos los meses simulados.");
                return false;
            }

            return true;
        }

        public double InventarioEnProceso()
        {
            double total = 0;
            for (int f = 0; f < Filas; f++)
                for (int c = 0; c < Columnas; c++)
                    if (Parcelas[f, c].EstaOcupada)
                        total += Parcelas[f, c].IngresosCosecha;
            return total;
        }

        public void MostrarInventario()
        {
            Console.WriteLine("\n  Inventario de semillas:");
            for (int i = 0; i < NombresCultivos.Length; i++)
                Console.WriteLine($"    {i + 1}. {NombresCultivos[i],-12}: {InventarioSemillas[i]} ud(s) | Costo: Q{CostosCultivos[i]} | Ingreso: Q{IngresosCultivos[i]} | Meses: {MesesCultivos[i]}");
        }

        public void MostrarReporteFinal()
        {
            double inventarioProceso = InventarioEnProceso();
            double utilidades = CapitalInicial + TotalIngresos + inventarioProceso - TotalManoObra - TotalMateriaPrima;

            Console.WriteLine("\n========================================");
            Console.WriteLine("        REPORTE FINAL DE GRANJA         ");
            Console.WriteLine("========================================");
            Console.WriteLine($"  Capital inicial       : Q{CapitalInicial:F2}");
            Console.WriteLine($"  Ingresos por cosecha  : Q{TotalIngresos:F2}");
            Console.WriteLine($"  Inventario en proceso : Q{inventarioProceso:F2}");
            Console.WriteLine($"  Mano de obra pagada   : Q{TotalManoObra:F2}");
            Console.WriteLine($"  Materia prima gastada : Q{TotalMateriaPrima:F2}");
            Console.WriteLine($"  Utilidades finales    : Q{utilidades:F2}");
            Console.WriteLine("========================================\n");
        }

        public string GetNombreCultivo(int idx) => NombresCultivos[idx];
        public double GetCostoCultivo(int idx) => CostosCultivos[idx];
        public double GetIngresoCultivo(int idx) => IngresosCultivos[idx];
        public int GetMesesCultivo(int idx) => MesesCultivos[idx];
    }
}