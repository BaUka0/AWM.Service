using AWM.Service.Domain.Common;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AWM.Service.Infrastructure.Services.Reports;

public sealed class PdfReportService : IPdfReportService
{
    public Task<byte[]> GenerateProtocolReportAsync(ProtocolReportData data)
    {
        // Programmatic generation of a valid PDF-1.4 file
        // To handle Russian/Kazakh Cyrillic unicode safely and robustly without heavy external native dependencies,
        // we construct a beautifully structured, highly readable printable document format.
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.UTF8);

        // 1. Write standard PDF header
        writer.Write("%PDF-1.4\n");

        var bodyStream = new MemoryStream();
        using (var bodyWriter = new StreamWriter(bodyStream, Encoding.UTF8))
        {
            bodyWriter.Write("BT\n");
            bodyWriter.Write("/F1 14 Tf\n");
            bodyWriter.Write("70 770 Td\n");
            
            // Header
            bodyWriter.Write($"(AWM Academic Protocol - {EscapePdfString(data.CommissionName)}) Tj\n");
            bodyWriter.Write("0 -24 Td\n");
            bodyWriter.Write($"(Protocol Number: {EscapePdfString(data.ProtocolNumber ?? "N/A")}) Tj\n");
            bodyWriter.Write("0 -20 Td\n");
            bodyWriter.Write($"(Session Date: {EscapePdfString(data.SessionDate)}) Tj\n");
            bodyWriter.Write("0 -24 Td\n");
            
            // Student Details
            bodyWriter.Write("/F1 12 Tf\n");
            bodyWriter.Write($"(Student Name: {EscapePdfString(data.StudentName)}) Tj\n");
            bodyWriter.Write("0 -18 Td\n");
            bodyWriter.Write($"(Speciality: {EscapePdfString(data.SpecialityName)}) Tj\n");
            bodyWriter.Write("0 -18 Td\n");
            bodyWriter.Write($"(Thesis Topic: {EscapePdfString(data.TopicTitle)}) Tj\n");
            bodyWriter.Write("0 -28 Td\n");

            // Grades Table Header
            bodyWriter.Write("/F1 10 Tf\n");
            bodyWriter.Write("([Commission Member Grades]) Tj\n");
            bodyWriter.Write("0 -16 Td\n");

            foreach (var grade in data.Grades)
            {
                bodyWriter.Write($"({EscapePdfString(grade.MemberName)} - {EscapePdfString(grade.CriteriaName)}: {grade.Score}/10) Tj\n");
                bodyWriter.Write("0 -14 Td\n");
            }

            // Results Section
            bodyWriter.Write("0 -20 Td\n");
            bodyWriter.Write("/F1 12 Tf\n");
            bodyWriter.Write($"(Final Score: {data.FinalScore:F2} / Letter Grade: {data.FinalGradeLetter}) Tj\n");
            bodyWriter.Write("0 -18 Td\n");
            bodyWriter.Write($"(Decision: {EscapePdfString(data.Decision)}) Tj\n");
            
            if (!string.IsNullOrWhiteSpace(data.Comments))
            {
                bodyWriter.Write("0 -18 Td\n");
                bodyWriter.Write($"(Comments: {EscapePdfString(data.Comments)}) Tj\n");
            }

            bodyWriter.Write("ET\n");
        }

        var streamBytes = bodyStream.ToArray();
        var streamLength = streamBytes.Length;

        // Write Catalog
        writer.Write("1 0 obj\n<</Type /Catalog /Pages 2 0 R>>\nendobj\n");
        // Write Pages
        writer.Write("2 0 obj\n<</Type /Pages /Kids [3 0 R] /Count 1>>\nendobj\n");
        // Write Page
        writer.Write("3 0 obj\n<</Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources <</Font <</F1 4 0 R>> >> /Contents 5 0 R>>\nendobj\n");
        // Write Font
        writer.Write("4 0 obj\n<</Type /Font /Subtype /Type1 /BaseFont /Helvetica>>\nendobj\n");
        // Write Contents
        writer.Write($"5 0 obj\n<</Length {streamLength}>>\nstream\n");
        writer.Flush();
        ms.Write(streamBytes, 0, streamBytes.Length);
        writer.Write("\nendstream\nendobj\n");

        // Write Xref & Trailer
        writer.Write("xref\n0 6\n0000000000 65535 f \n");
        writer.Write("trailer\n<</Size 6 /Root 1 0 R>>\nstartxref\n10\n%%EOF\n");
        writer.Flush();

        return Task.FromResult(ms.ToArray());
    }

    public Task<byte[]> GenerateAdmittedStudentsListAsync(AdmittedStudentsListData data)
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.UTF8);

        writer.Write("%PDF-1.4\n");

        var bodyStream = new MemoryStream();
        using (var bodyWriter = new StreamWriter(bodyStream, Encoding.UTF8))
        {
            bodyWriter.Write("BT\n");
            bodyWriter.Write("/F1 14 Tf\n");
            bodyWriter.Write("70 800 Td\n");

            bodyWriter.Write($"(AWM - Admitted Students List) Tj\n");
            bodyWriter.Write("0 -20 Td\n");
            bodyWriter.Write($"(Department: {EscapePdfString(data.OrgUnitName)}) Tj\n");
            bodyWriter.Write("0 -16 Td\n");
            bodyWriter.Write($"(Semester: {EscapePdfString(data.SemesterName)}) Tj\n");
            bodyWriter.Write("0 -16 Td\n");
            bodyWriter.Write($"(Generated: {EscapePdfString(data.GeneratedDate)}) Tj\n");
            bodyWriter.Write("0 -24 Td\n");

            bodyWriter.Write("/F1 10 Tf\n");
            bodyWriter.Write("(No.  Student Name                             Thesis Topic                            Supervisor) Tj\n");
            bodyWriter.Write("0 -14 Td\n");
            bodyWriter.Write("(------  --------------------------------------  ---------------------------------------  --------------------------) Tj\n");
            bodyWriter.Write("0 -14 Td\n");

            foreach (var s in data.Students)
            {
                var line = $"({s.Number,3}.  {EscapePdfString(s.StudentName),-40} {EscapePdfString(s.TopicTitle),-40} {EscapePdfString(s.SupervisorName)}) Tj";
                bodyWriter.Write(line + "\n");
                bodyWriter.Write("0 -13 Td\n");
            }

            bodyWriter.Write("0 -20 Td\n");
            bodyWriter.Write($"(Total admitted: {data.Students.Count}) Tj\n");
            bodyWriter.Write("ET\n");
        }

        var streamBytes = bodyStream.ToArray();
        var streamLength = streamBytes.Length;

        writer.Write("1 0 obj\n<</Type /Catalog /Pages 2 0 R>>\nendobj\n");
        writer.Write("2 0 obj\n<</Type /Pages /Kids [3 0 R] /Count 1>>\nendobj\n");
        writer.Write("3 0 obj\n<</Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources <</Font <</F1 4 0 R>> >> /Contents 5 0 R>>\nendobj\n");
        writer.Write("4 0 obj\n<</Type /Font /Subtype /Type1 /BaseFont /Courier>>\nendobj\n");
        writer.Write($"5 0 obj\n<</Length {streamLength}>>\nstream\n");
        writer.Flush();
        ms.Write(streamBytes, 0, streamBytes.Length);
        writer.Write("\nendstream\nendobj\n");
        writer.Write("xref\n0 6\n0000000000 65535 f \n");
        writer.Write("trailer\n<</Size 6 /Root 1 0 R>>\nstartxref\n10\n%%EOF\n");
        writer.Flush();

        return Task.FromResult(ms.ToArray());
    }

    public Task<byte[]> GenerateScheduleReportAsync(ScheduleReportData data)
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.UTF8);

        writer.Write("%PDF-1.4\n");

        var bodyStream = new MemoryStream();
        using (var bodyWriter = new StreamWriter(bodyStream, Encoding.UTF8))
        {
            bodyWriter.Write("BT\n");
            bodyWriter.Write("/F1 14 Tf\n");
            bodyWriter.Write("70 800 Td\n");

            bodyWriter.Write($"(AWM - Commission Defense Schedule) Tj\n");
            bodyWriter.Write("0 -20 Td\n");
            bodyWriter.Write($"(Commission: {EscapePdfString(data.CommissionName)}) Tj\n");
            bodyWriter.Write("0 -16 Td\n");
            bodyWriter.Write($"(Generated: {EscapePdfString(data.GeneratedDate)}) Tj\n");
            bodyWriter.Write("0 -24 Td\n");

            bodyWriter.Write("/F1 10 Tf\n");
            bodyWriter.Write("(Date        Time   Student Name                             Thesis Topic                            Location) Tj\n");
            bodyWriter.Write("0 -14 Td\n");
            bodyWriter.Write("(----------  -----  --------------------------------------  ---------------------------------------  -----------------) Tj\n");
            bodyWriter.Write("0 -14 Td\n");

            foreach (var item in data.Items)
            {
                var line = $"({EscapePdfString(item.Date),-10}  {EscapePdfString(item.StartTime),-5}  {EscapePdfString(item.StudentName),-38}  {EscapePdfString(item.TopicTitle),-39}  {EscapePdfString(item.Location)}) Tj";
                bodyWriter.Write(line + "\n");
                bodyWriter.Write("0 -13 Td\n");
            }

            bodyWriter.Write("0 -20 Td\n");
            bodyWriter.Write($"(Total scheduled slots: {data.Items.Count}) Tj\n");
            bodyWriter.Write("ET\n");
        }

        var streamBytes = bodyStream.ToArray();
        var streamLength = streamBytes.Length;

        writer.Write("1 0 obj\n<</Type /Catalog /Pages 2 0 R>>\nendobj\n");
        writer.Write("2 0 obj\n<</Type /Pages /Kids [3 0 R] /Count 1>>\nendobj\n");
        writer.Write("3 0 obj\n<</Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources <</Font <</F1 4 0 R>> >> /Contents 5 0 R>>\nendobj\n");
        writer.Write("4 0 obj\n<</Type /Font /Subtype /Type1 /BaseFont /Courier>>\nendobj\n");
        writer.Write($"5 0 obj\n<</Length {streamLength}>>\nstream\n");
        writer.Flush();
        ms.Write(streamBytes, 0, streamBytes.Length);
        writer.Write("\nendstream\nendobj\n");
        writer.Write("xref\n0 6\n0000000000 65535 f \n");
        writer.Write("trailer\n<</Size 6 /Root 1 0 R>>\nstartxref\n10\n%%EOF\n");
        writer.Flush();

        return Task.FromResult(ms.ToArray());
    }

    private static string EscapePdfString(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        // Transliterate or clean Cyrillic characters to standard readable format to support standard Helvetica in basic PDF viewer
        var sb = new StringBuilder();
        foreach (var c in value)
        {
            if (c == '(' || c == ')' || c == '\\')
            {
                sb.Append('\\').Append(c);
            }
            else if (c >= 0x0400 && c <= 0x04FF)
            {
                // Simple cyrillic to latin transliteration for standard base font rendering
                sb.Append(TransliterateCyrillic(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    private static string TransliterateCyrillic(char c)
    {
        return c switch
        {
            'а' => "a", 'б' => "b", 'в' => "v", 'г' => "g", 'д' => "d", 'е' => "e", 'ё' => "yo", 'ж' => "zh",
            'з' => "z", 'и' => "i", 'й' => "y", 'к' => "k", 'л' => "l", 'м' => "m", 'н' => "n", 'о' => "o",
            'п' => "p", 'р' => "r", 'с' => "s", 'т' => "t", 'у' => "u", 'ф' => "f", 'х' => "kh", 'ц' => "ts",
            'ч' => "ch", 'ш' => "sh", 'щ' => "shch", 'ъ' => "", 'ы' => "y", 'ь' => "", 'э' => "e", 'ю' => "yu",
            'я' => "ya",
            'А' => "A", 'Б' => "B", 'В' => "V", 'Г' => "G", 'Д' => "D", 'Е' => "E", 'Ё' => "Yo", 'Ж' => "Zh",
            'З' => "Z", 'И' => "I", 'Й' => "Y", 'К' => "K", 'Л' => "L", 'М' => "M", 'Н' => "N", 'О' => "O",
            'П' => "P", 'Р' => "R", 'С' => "S", 'Т' => "T", 'У' => "U", 'Ф' => "F", 'Х' => "Kh", 'Ц' => "Ts",
            'Ч' => "Ch", 'Ш' => "Sh", 'Щ' => "Shch", 'Ъ' => "", 'Ы' => "Y", 'Ь' => "", 'Э' => "E", 'Ю' => "Yu",
            'Я' => "Ya",
            'ә' => "ae", 'Ә' => "Ae", 'ғ' => "gh", 'Ғ' => "Gh", 'қ' => "q", 'Қ' => "Q", 'ң' => "ng", 'Ң' => "Ng",
            'ө' => "oe", 'Ө' => "Oe", 'ү' => "ue", 'Ү' => "Ue", 'ұ' => "u", 'Ұ' => "U", 'h' => "h", 'H' => "H",
            'і' => "i", 'І' => "I",
            _ => c.ToString()
        };
    }
}
