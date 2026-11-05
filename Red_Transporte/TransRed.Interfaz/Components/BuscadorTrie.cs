
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Components;

// Autocompletado de paradas utilizando el Trie del Core
public static class BuscadorTrie
{
    public static void Activar(
        ComboBox combo,
        IGestorTransporte gestor)
    {
        // Permite escribir en el selector
        combo.DropDownStyle = ComboBoxStyle.DropDown;
        combo.AutoCompleteMode = AutoCompleteMode.None;
        combo.AutoCompleteSource = AutoCompleteSource.None;

        combo.FlatStyle = FlatStyle.Flat;
        combo.BackColor = TransRedTheme.Superficie;
        combo.ForeColor = TransRedTheme.Tinta;
        combo.Font = new Font("Segoe UI", 9.5f);

        combo.IntegralHeight = false;
        combo.DropDownHeight = 260;
        combo.DropDownWidth = Math.Max(360, combo.Width);

        bool actualizando = false;

        // Mantiene disponibles todas las paradas
        Parada[] todas = gestor.ObtenerTodasParadas();

        combo.TextUpdate += (_, _) =>
        {
            if (actualizando)
                return;

            actualizando = true;

            try
            {
                string texto = combo.Text;
                int posicion = combo.SelectionStart;

                Parada[] sugerencias;

                if (string.IsNullOrWhiteSpace(texto))
                {
                    // Muestra todas las paradas si no hay búsqueda
                    sugerencias = todas;
                }
                else
                {
                    // La búsqueda por nombre la realiza el Trie
                    sugerencias = gestor.SugerirParadas(
                        texto.Trim());

                    // También permite localizar por código
                    if (sugerencias.Length == 0)
                    {
                        sugerencias = todas
                            .Where(p =>
                                p.Codigo.StartsWith(
                                    texto.Trim(),
                                    StringComparison.OrdinalIgnoreCase))
                            .ToArray();
                    }
                }

                // Actualiza las opciones sin perder lo escrito
                combo.BeginUpdate();

                try
                {
                    combo.Items.Clear();

                    foreach (Parada parada in sugerencias)
                        combo.Items.Add(parada);
                }
                finally
                {
                    combo.EndUpdate();
                }

                combo.Text = texto;
                combo.SelectionStart = Math.Min(
                    posicion, combo.Text.Length);

                combo.SelectionLength = 0;

                // Abre las sugerencias al escribir
                combo.DroppedDown = sugerencias.Length > 0;

                if (combo.DroppedDown)
                    Cursor.Current = Cursors.IBeam;
            }
            finally
            {
                actualizando = false;
            }
        };

        // Permite confirmar una sugerencia con Enter
        combo.KeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.Enter)
                return;

            if (combo.SelectedItem is not Parada &&
                combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }

            combo.DroppedDown = false;

            e.Handled = true;
            e.SuppressKeyPress = true;
        };

        // Conserva el objeto Parada seleccionado
        combo.SelectionChangeCommitted += (_, _) =>
        {
            combo.DroppedDown = false;
        };
    }
}
