using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Uniconta.ClientTools.DataModel;
using System.Threading;
using System.Text;
using System.Net;

public class TaxEntry
{
	public double TaxPct { get; set; }
    public string TaxCode { get; set; }
    public double TaxAmount { get; set; }
    public double TaxableAmount { get; set; }
}

// Adds a new tax entry to the list if the tax code does not exist.
// If the tax code exists, updates its TaxAmount, TaxableAmount, and TaxPct.
// Matching is case-insensitive, and empty/null codes are replaced with "U0".
public void AddOrUpdateTax(List<TaxEntry> taxes, string taxCode, double taxAmount, double taxableAmount, double pct)
{
    var entry = taxes.FirstOrDefault<TaxEntry>(
        t => string.Equals(t.TaxCode, taxCode, StringComparison.OrdinalIgnoreCase));

	if (string.IsNullOrEmpty(taxCode))
		taxCode = "U0";

    if (entry != null)
    {
        entry.TaxAmount += taxAmount;
        entry.TaxableAmount += taxableAmount;
		entry.TaxPct = pct;
    }
    else
    {
        taxes.Add(new TaxEntry
        {
            TaxCode = taxCode,
            TaxAmount = taxAmount,
            TaxableAmount = taxableAmount,
			TaxPct = pct
        });
    }
}

// Finds the first tax entry in the list matching the given tax code (case-insensitive).
// Returns null if not found.
public TaxEntry FindTax(List<TaxEntry> taxes, string taxCode)
{
    return taxes.FirstOrDefault<TaxEntry>(t => string.Equals(t.TaxCode, taxCode, StringComparison.OrdinalIgnoreCase));
}

string hvadd = "";

// Event handler for the "BeforePrint" event of the total sum footer in a Uniconta report.
// Builds a list of tax entries from invoice lines, then formats and assigns the summary text to label2.
private void ReportTotalSumFooter_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e) {

	Thread.CurrentThread.CurrentCulture = new CultureInfo("is-IS");
	var taxList = new List<TaxEntry>();
	var colSource = GetCurrentColumnValue("DebtorInfo") as DebtorClient;

	if (colSource != null)
	{
		var invLines = colSource.InventoryTransInvoice as InvTransInvoice[];
		foreach(var invLine in invLines)
		{
			hvadd += invLine.Vat + ";"+invLine.VatPct+ "\n";
			AddOrUpdateTax(taxList, invLine.Vat, invLine.VatAmount, (-1)*invLine.NetAmount, invLine.VatPct);
		}
	}

	label2.Text = RenderTaxListText(taxList);
}

// Renders a plain text summary of the provided tax entries.
// The output is a table-like format using spaces/tabs for alignment and '\n' for new lines.
// Includes per-code rows and a total row at the bottom.
public string RenderTaxListText(IEnumerable<TaxEntry> taxes)
{
	decimal totalTaxable = 0m;
	decimal totalTax = 0m;

	const int col1Width = 10; // VSK kóði (+ pct)
	const int col2Width = 18; // Skattskyld upphæð
	const int col3Width = 12; // Skattur

	var sb = new StringBuilder();

	sb.Append("Samantekt á VSK\n");
	sb.Append(new string('-', col1Width + col2Width + col3Width + 14)).Append("\n");
	sb.Append("VSK kóði".PadRight(col1Width)).Append("\t")
	  .Append("Skattskyld upphæð".PadRight(col2Width)).Append("\t")
	  .Append("Skattur".PadRight(col3Width)).Append("\n");
	sb.Append(new string('-', col1Width + col2Width + col3Width + 14)).Append("\n");

	foreach (var t in taxes)
	{
		sb.Append((t.TaxCode + "  ("+t.TaxPct+"%)" ?? "").PadRight(col1Width)).Append("\t")
		  .Append(t.TaxableAmount.ToString("N2", CultureInfo.InvariantCulture).PadLeft(col2Width)).Append("\t")
		  .Append(t.TaxAmount.ToString("N2", CultureInfo.InvariantCulture).PadLeft(col3Width)).Append("\n");

		totalTaxable += (decimal)t.TaxableAmount;
		totalTax += (decimal)t.TaxAmount;
	}

	sb.Append(new string('-', col1Width + col2Width + col3Width + 14)).Append("\n");
	sb.Append("Samtals".PadRight(col1Width)).Append("\t")
	  .Append(totalTaxable.ToString("N2", CultureInfo.InvariantCulture).PadLeft(col2Width)).Append("\t")
	  .Append(totalTax.ToString("N2", CultureInfo.InvariantCulture).PadLeft(col3Width)).Append("\n");

	return sb.ToString();
}
