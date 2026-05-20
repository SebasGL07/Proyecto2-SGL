using System;

namespace GranjaSimulacion
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("========================================");
            Console.WriteLine("     GESTION DE GRANJA POR CONSOLA     ");
            Console.WriteLine("   Universidad Rafael Landivar - PC    ");
            Console.WriteLine("========================================\n");

            double dineroInicial = LeerPositivo("Ingrese el dinero inicial de la granja (Q): ");
            int empleados = (int)LeerPositivo("Ingrese el numero de empleados: ");
            double sueldo = LeerPositivo("Ingrese el sueldo mensual por empleado (Q): ");
            int meses = (int)LeerPositivo("Ingrese la cantidad de meses a simular: ");
            int filas = (int)LeerPositivo("Ingrese el numero de filas de parcelas: ");
            int columnas = (int)LeerPositivo("Ingrese el numero de columnas de parcelas: ");

            Granja granja = new Granja(dineroInicial, empleados, sueldo, meses, filas, columnas);

            Console.WriteLine("\n  Granja creada! La simulacion ha comenzado.\n");

            bool continuar = true;

            while (continuar)
            {
                MostrarMenuPrincipal(granja);
                int opcion = LeerOpcion(1, 5);

                switch (opcion)
                {
                    case 1: MenuComprarSemillas(granja); break;
                    case 2: MenuSembrar(granja); break;
                    case 3: MenuConsultarParcela(granja); break;
                    case 4: continuar = granja.AvanzarMes(); break;
                    case 5:
                        Console.WriteLine("\n  Terminando simulacion...");
                        continuar = false;
                        break;
                }
            }

            granja.MostrarReporteFinal();

            Console.WriteLine("  Presione cualquier tecla para salir...");
            Console.ReadKey();
        }

        static void MostrarMenuPrincipal(Granja granja)
        {
            Console.WriteLine("\n----------------------------------------");
            Console.WriteLine($"  Caja: Q{granja.Dinero:F2}  |  Mes(es) restante(s): {granja.MesesRestantes}");
            Console.WriteLine("  1. Comprar semillas");
            Console.WriteLine("  2. Sembrar en parcela");
            Console.WriteLine("  3. Consultar parcela");
            Console.WriteLine("  4. Avanzar de mes");
            Console.WriteLine("  5. Salir (ver reporte final)");
            Console.Write("  Elige una opcion: ");
        }

        static void MenuComprarSemillas(Granja granja)
        {
            double utilidad = granja.UtilidadProyectada();

            if (utilidad < 0)
            {
                Console.WriteLine($"\n  [!] No puedes comprar semillas.");
                Console.WriteLine($"      Utilidad proyectada negativa: Q{utilidad:F2}");
                Console.WriteLine($"      (Caja Q{granja.Dinero:F2} - Costos Q{granja.CostosMensuales():F2})");
                return;
            }

            Console.WriteLine($"\n  Caja actual          : Q{granja.Dinero:F2}");
            Console.WriteLine($"  Costos mens. proyect.: Q{granja.CostosMensuales():F2}");
            Console.WriteLine($"  Utilidad proyectada  : Q{utilidad:F2}");
            granja.MostrarInventario();

            Console.Write("\n  Tipo de semilla a comprar (1-5): ");
            int tipoIdx = LeerOpcion(1, 5) - 1;

            Console.Write($"  Cantidad de semillas de {granja.GetNombreCultivo(tipoIdx)} (Q{granja.GetCostoCultivo(tipoIdx)} c/u): ");
            int cantidad = (int)LeerPositivo("");

            granja.ComprarSemillas(tipoIdx, cantidad);
        }

        static void MenuSembrar(Granja granja)
        {
            // Primero se elige el tipo de siembra, luego la celda
            granja.MostrarInventario();

            Console.Write("\n  Tipo de semilla a sembrar (1-5): ");
            int tipoIdx = LeerOpcion(1, 5) - 1;

            if (granja.InventarioSemillas[tipoIdx] <= 0)
            {
                Console.WriteLine($"\n  [!] No tienes semillas de {granja.GetNombreCultivo(tipoIdx)}. Compra primero.");
                return;
            }

            Console.WriteLine($"  Sembrando: {granja.GetNombreCultivo(tipoIdx)} " +
                              $"(madura en {granja.GetMesesCultivo(tipoIdx)} mes(es), ingreso: Q{granja.GetIngresoCultivo(tipoIdx)})");

            granja.MostrarMapa();

            int fila = (int)LeerRango($"  Fila (1-{granja.Filas}): ", 1, granja.Filas) - 1;
            int columna = (int)LeerRango($"  Columna (1-{granja.Columnas}): ", 1, granja.Columnas) - 1;

            granja.Sembrar(fila, columna, tipoIdx);
        }

        static void MenuConsultarParcela(Granja granja)
        {
            granja.MostrarMapa();
            int fila = (int)LeerRango($"  Fila a consultar (1-{granja.Filas}): ", 1, granja.Filas) - 1;
            int columna = (int)LeerRango($"  Columna a consultar (1-{granja.Columnas}): ", 1, granja.Columnas) - 1;
            granja.ConsultarParcela(fila, columna);
        }

        static double LeerPositivo(string mensaje)
        {
            double valor;
            while (true)
            {
                if (!string.IsNullOrEmpty(mensaje))
                    Console.Write(mensaje);

                if (double.TryParse(Console.ReadLine(), out valor) && valor > 0)
                    return valor;

                Console.WriteLine("  [!] Ingrese un numero valido mayor a 0.");
            }
        }

        static int LeerOpcion(int min, int max)
        {
            int valor;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out valor) && valor >= min && valor <= max)
                    return valor;

                Console.Write($"  [!] Opcion invalida. Ingrese entre {min} y {max}: ");
            }
        }

        static double LeerRango(string mensaje, int min, int max)
        {
            int valor;
            while (true)
            {
                Console.Write(mensaje);
                if (int.TryParse(Console.ReadLine(), out valor) && valor >= min && valor <= max)
                    return valor;

                Console.WriteLine($"  [!] Ingrese un valor entre {min} y {max}.");
            }
        }
    }
}