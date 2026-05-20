using System;

namespace GranjaSimulacion
{
    class Parcela
    {
        public string TipoSiembra { get; set; }
        public int MesesParaCrecer { get; set; }
        public int MesesRestantes { get; set; }
        public double IngresosCosecha { get; set; }
        public bool EstaOcupada { get; set; }

        public Parcela()
        {
            TipoSiembra = "Vacia";
            MesesParaCrecer = 0;
            MesesRestantes = 0;
            IngresosCosecha = 0;
            EstaOcupada = false;
        }

        public void Limpiar()
        {
            TipoSiembra = "Vacia";
            MesesParaCrecer = 0;
            MesesRestantes = 0;
            IngresosCosecha = 0;
            EstaOcupada = false;
        }
    }
}