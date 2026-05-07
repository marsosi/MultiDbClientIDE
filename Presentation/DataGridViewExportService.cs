using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MultiDbClientIDE.Presentation
{
	public static class DataGridViewExportService
	{
		public static void ExportToExcel(DataGridView dgv)
		{
			var excelApp = new Microsoft.Office.Interop.Excel.Application();
			var workbook = excelApp.Workbooks.Add(Type.Missing);
			Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.ActiveSheet;
			worksheet.Name = "Exportado do DataGridView";
			for (int i = 1; i <= dgv.Columns.Count; i++)
				worksheet.Cells[1, i] = dgv.Columns[i - 1].HeaderText;
			for (int i = 0; i < dgv.Rows.Count; i++)
			{
				for (int j = 0; j < dgv.Columns.Count; j++)
					worksheet.Cells[i + 2, j + 1] = dgv.Rows[i].Cells[j].Value?.ToString();
			}
			excelApp.Visible = true;
		}

		public static void ExportToCsv(DataGridView dgv, string filePath)
		{
			using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
			{
				for (int i = 0; i < dgv.Columns.Count; i++)
				{
					sw.Write(dgv.Columns[i].HeaderText);
					if (i < dgv.Columns.Count - 1)
						sw.Write(";");
				}
				sw.WriteLine();
				foreach (DataGridViewRow row in dgv.Rows)
				{
					if (row.IsNewRow) continue;
					for (int i = 0; i < dgv.Columns.Count; i++)
					{
						sw.Write(row.Cells[i].Value?.ToString());
						if (i < dgv.Columns.Count - 1)
							sw.Write(";");
					}
					sw.WriteLine();
				}
			}
		}
	}
}
