using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Globalization;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace CoreClass
{
	/// <summary>
	/// 文件操作类
	/// </summary>
	public class OperateFileClass
	{
		public string m_webrootpath = HttpContext.HostingEnvironment.WebRootPath;

		/// <summary>
		/// 获取一个绝对不重复的文件名
		/// </summary>
		public string g_FileSaveName
		{
			get
			{
				return Guid.NewGuid().ToString("N");
			}
		}
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="imgFile">文件对象</param>
        /// <param name="dirPath">目标位置(格式：a/b/)</param>
        /// <param name="filetype">允许的上传类型("|"分隔)，null为不限</param>
        /// <param name="uploadsize">最大允许上传量(字节 B)，0为不限</param>
        /// <returns>文件名</returns>
        public string UploadFile(IFormFile imgFile, string dirPath, string filetype, long uploadsize)
        {
            //定义允许上传的文件扩展名
            string FILE_TYPE = filetype;
            //最大文件大小
            long MAX_SIZE = uploadsize;
            if (imgFile == null)
            {
                throw new Exception("未选择文件。");
            }
			string rootPath = Path.Combine(m_webrootpath, dirPath);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            string fileName = imgFile.FileName;
            string fileExt = Path.GetExtension(fileName).ToLower();
			if (imgFile == null || (MAX_SIZE != 0 && imgFile.Length > MAX_SIZE))
            {
                throw new Exception("上传文件大小超过限制。");
            }
            if (string.IsNullOrEmpty(fileExt) || (FILE_TYPE != null && Array.IndexOf(FILE_TYPE.Split('|'), fileExt.Substring(1).ToLower()) == -1))
            {
                throw new Exception("上传文件格式被禁止。");
            }
            string filePath = "";
            string newFileName = null;
            try
            {
                newFileName = g_FileSaveName + fileExt;
                filePath = rootPath + newFileName;
                while (File.Exists(filePath))
                {
                    newFileName = g_FileSaveName + fileExt;
                    filePath = rootPath + newFileName;
                }
				using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
				{
					imgFile.CopyTo(fileStream);
				}
            }
            catch
            {
                throw new Exception("未知错误，上传文件失败。");
            }
            return newFileName;
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="imgFile">文件对象</param>
        /// <param name="remoteAddress">远程地址(格式：//192.168.1.1/)</param>
        /// <param name="dirPath">目标位置(格式：a/b/)</param>
        /// <param name="filetype">允许的上传类型("|"分隔)，null为不限</param>
        /// <param name="uploadsize">最大允许上传量(字节 B)，0为不限</param>
        /// <returns>文件名</returns>
        public string UploadFile(IFormFile imgFile, string remoteAddress, string dirPath, string filetype, long uploadsize)
        {
            //定义允许上传的文件扩展名
            string FILE_TYPE = filetype;
            //最大文件大小
            long MAX_SIZE = uploadsize;
            if (imgFile == null)
            {
                throw new Exception("未选择文件。");
            }
            string rootPath = remoteAddress + dirPath;
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            string fileName = imgFile.FileName;
            string fileExt = Path.GetExtension(fileName).ToLower();
            if (imgFile == null || (MAX_SIZE != 0 && imgFile.Length > MAX_SIZE))
            {
                throw new Exception("上传文件大小超过限制。");
            }
            if (string.IsNullOrEmpty(fileExt) || (FILE_TYPE != null && Array.IndexOf(FILE_TYPE.Split('|'), fileExt.Substring(1).ToLower()) == -1))
            {
                throw new Exception("上传文件格式被禁止。");
            }
            string filePath = "";
            string newFileName = "";
            try
            {
                newFileName = g_FileSaveName + fileExt;
                filePath = rootPath + newFileName;
                while (File.Exists(filePath))
                {
                    newFileName = g_FileSaveName + fileExt;
                    filePath = rootPath + newFileName;
                }
				using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
				{
					imgFile.CopyTo(fileStream);
				}
            }
            catch
            {
                throw new Exception("未知错误，上传文件失败。");
            }
            return newFileName;
        }
        /*
        ============图片处理模块============
        */
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="file">文件对象</param>
        /// <param name="ext">保存文件扩展名(jpg,jpeg,gif,bmp,png之一)</param>
        /// <param name="path">目标位置(格式：a/b/)</param>
        /// <param name="filename">文件名，为null则自动生成</param>
        /// <returns>文件名</returns>
        public string SaveImage(Bitmap file, string ext, string path, string filename)
        {
            string str = null;
            try
            {
                ext = ext.ToLower();
                string fileExt = "." + ext;
                if (filename != null)
                {
                    fileExt = filename + fileExt;
                }
                else
                {
                    fileExt = g_FileSaveName + fileExt;
				}
				path = Path.Combine(m_webrootpath, path);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + fileExt;
                switch (ext)
                {
                    case "bmp":
                        file.Save(@path, ImageFormat.Bmp);
                        break;
                    case "gif":
                        file.Save(@path, ImageFormat.Gif);
                        break;
                    case "jpg":
                        file.Save(@path, ImageFormat.Jpeg);
                        break;
                    case "jpeg":
                        file.Save(@path, ImageFormat.Jpeg);
                        break;
                    case "png":
                        file.Save(@path, ImageFormat.Png);
                        break;
                }
                str = fileExt;
            }
            catch (Exception e) { throw e; }
            return str;
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="file">文件对象</param>
        /// <param name="ext">保存文件扩展名(jpg,jpeg,gif,bmp,png之一)</param>
        /// <param name="remoteAddress">远程地址(格式：//192.168.1.1/)</param>
        /// <param name="path">目标位置(格式：a/b/)</param>
        /// <param name="filename">文件名，为null则自动生成</param>
        /// <returns>文件名</returns>
        public string SaveImage(Bitmap file, string ext, string remoteAddress, string path, string filename)
        {
            string str = null;
            try
            {
                ext = ext.ToLower();
                string fileExt = "." + ext;
                if (filename != null)
                {
                    fileExt = filename + fileExt;
                }
                else
                {
                    fileExt = g_FileSaveName + fileExt;
                }
                path = remoteAddress + path;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + fileExt;
                switch (ext)
                {
                    case "bmp":
                        file.Save(@path, ImageFormat.Bmp);
                        break;
                    case "gif":
                        file.Save(@path, ImageFormat.Gif);
                        break;
                    case "jpg":
                        file.Save(@path, ImageFormat.Jpeg);
                        break;
                    case "jpeg":
                        file.Save(@path, ImageFormat.Jpeg);
                        break;
                    case "png":
                        file.Save(@path, ImageFormat.Png);
                        break;
                }
                str = fileExt;
            }
            catch (Exception e) { throw e; }
            return str;
        }
		/// 缩放图片
		/// </summary>
		/// <param name="bmp">原始Bitmap</param>
		/// <param name="newW">新的宽度</param>
		/// <param name="newH">新的高度</param>
		/// <returns>处理以后的图片</returns>
		public Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
		{
			try
			{
				Bitmap b = new Bitmap(newW, newH);
				Graphics g = Graphics.FromImage(b);
				//插值算法的质量
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
				g.Dispose();
				return b;
			}
			catch
			{
				return null;
			}
		}
		/// 获取二维码
		/// </summary>
        /// <param name="content">内容</param>
        /// <param name="ModuleSize">大小（数值*25px）</param>
		/// <returns>二维码图片</returns>
        public Bitmap GetQRCode(string content, int ModuleSize)
		{
            Bitmap bitmap = null;
			try
			{
                //生成二维码
                using (MemoryStream ms = new MemoryStream())
                {
                    ErrorCorrectionLevel Ecl = ErrorCorrectionLevel.M; //误差校正水平 
                    string Content = content;//待编码内容
                    QuietZoneModules QuietZones = QuietZoneModules.Two;  //空白区域
                    var encoder = new QrEncoder(Ecl);
                    QrCode qr;
                    if (encoder.TryEncode(Content, out qr))//对内容进行编码，并保存生成的矩阵
                    {
                        var render = new GraphicsRenderer(new FixedModuleSize(ModuleSize, QuietZones));
                        render.WriteToStream(qr.Matrix, ImageFormat.Png, ms);
                    }
                    bitmap = new Bitmap(ms);
                }
			}
			catch(Exception e)
			{
                throw e;
            }
            return bitmap;
		}
		/*
		============文件处理模块============
		*/
		/// <summary>
		/// 读取字符文件
		/// </summary>
		/// <param name="path">文件路径(格式：a/b/a.txt)</param>
		/// <returns>文件内容</returns>
		public string ReadFile(string path)
		{
			path = Path.Combine(m_webrootpath, path);
			StreamReader sr = new StreamReader(path, Encoding.UTF8);
			string str = sr.ReadToEnd();
			sr.Dispose();
			sr.Close();
			return str;
		}
		/// <summary>
		/// 写入字符文件
		/// </summary>
		/// <param name="content">写入内容</param>
		/// <param name="path">文件路径(格式：a/b/a.txt)</param>
		/// <returns>布尔</returns>
		public bool WriteFile(string content, string path)
		{
			bool b = false;
			try
			{
				path = Path.Combine(m_webrootpath, path);
				StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
				sw.Write(content);
				sw.Dispose();
				sw.Close();
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 移动文件
		/// </summary>
		/// <param name="path">源文件路径(格式：a/b/a.txt)</param>
		/// <param name="movetopath">目标文件路径(格式：a/b/a.txt)</param>
		/// <returns>布尔</returns>
		public bool MoveFile(string path, string movetopath)
		{
			bool b = false;
			try
			{
				path = Path.Combine(m_webrootpath, path);
				movetopath = Path.Combine(m_webrootpath, movetopath);
				File.Move(path, movetopath);
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 复制文件
		/// </summary>
		/// <param name="path">源文件路径(格式：a/b/a.txt)</param>
        /// <param name="movetopath">目标文件路径(格式：a/b/a.txt)</param>
		/// <returns>布尔</returns>
		public bool CopyFile(string path, string movetopath)
		{
			bool b = false;
			try
			{
				if (!Directory.Exists(movetopath))
				{
					Directory.CreateDirectory(movetopath);
				}
				path = Path.Combine(m_webrootpath, path);
				movetopath = Path.Combine(m_webrootpath, movetopath);
				File.Copy(path, movetopath, true);
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 删除文件
		/// </summary>
		/// <param name="path">源文件路径(格式：a/b/a.txt)</param>
		/// <returns>布尔</returns>
		public bool DelFile(string path)
		{
			bool b = false;
			try
			{
				path = Path.Combine(m_webrootpath, path);
				File.Delete(path);
				b = true;
			}
			catch (Exception e) {  }
			return b;
		}
		/// <summary>
		/// 删除文件
        /// </summary>
        /// <param name="remoteAddress">远程地址(格式：//192.168.1.1/)</param>
		/// <param name="path">源文件路径(格式：a/b/a.txt)</param>
		/// <returns>布尔</returns>
		public bool DelFile(string remoteAddress, string path)
		{
			bool b = false;
			try
			{
                File.Delete(remoteAddress + path);
				b = true;
			}
			catch (Exception e) { }
			return b;
		}
        /*
        ============Excel处理模块============
        */
        /// <summary>  
        /// 将Excel文件中的数据读出到DataTable中(xls)  
        /// </summary>  
        /// <param name="file">物理路径</param>
        /// <param name="isFormulaToNumeric"></param>  
        /// <returns></returns>  
        public DataTable ExcelToTableForXLS(string file, bool isFormulaToNumeric = true)
        {
            DataTable dt = new DataTable();
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    dt = ExcelToTableForXLS(fs);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return dt;
        }
        /// <summary>  
        /// 将Excel文件中的数据读出到DataTable中(xls)  
        /// </summary>  
        /// <param name="file">文件流</param>
        /// <param name="isFormulaToNumeric"></param>  
        /// <returns></returns>  
        public DataTable ExcelToTableForXLS(Stream file, bool isFormulaToNumeric = true)
        {
            DataTable dt = new DataTable();
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
            ISheet sheet = hssfworkbook.GetSheetAt(0);

            //表头  
            IRow header = sheet.GetRow(sheet.FirstRowNum);
            List<int> columns = new List<int>();
            for (int i = 0; i < header.LastCellNum; i++)
            {
                object obj = GetValueTypeForXLS(header.GetCell(i) as HSSFCell, isFormulaToNumeric);
                if (obj == null || obj.ToString() == string.Empty)
                {
                    dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    //continue;  
                }
                else
                    dt.Columns.Add(new DataColumn(obj.ToString()));
                columns.Add(i);
            }
            //数据  
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                DataRow dr = dt.NewRow();
                bool hasValue = false;
                foreach (int j in columns)
                {
                    dr[j] = GetValueTypeForXLS(sheet.GetRow(i).GetCell(j) as HSSFCell, isFormulaToNumeric);
                    if (dr[j] != null && dr[j].ToString() != string.Empty)
                    {
                        hasValue = true;
                    }
                }
                if (hasValue)
                {
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        /// <summary>  
		/// 将DataTable数据导出到Excel文件中(xls)  
		/// </summary>  
		/// <param name="dt"></param>  
		/// <param name="file">物理路径</param>  
		public void TableToExcelForXLS(DataTable dt, string file)
        {
            MemoryStream stream = (MemoryStream)TableToExcelForXLS(dt);
            byte[] buf = stream.ToArray();
            //保存为Excel文件  
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>  
        /// 将DataTable数据导出到Excel文件中(xls)  
        /// </summary>  
        /// <param name="dt"></param>
        public Stream TableToExcelForXLS(DataTable dt)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("Test");

            //表头  
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //宽度自适应
            if (dt.Columns.Count > 26)
            {
                throw new Exception("列数超出Excel允许的最大值：26");
            }
            for (int columnNum = 0; columnNum < dt.Columns.Count; columnNum++)
            {
                int columnWidth = sheet.GetColumnWidth(columnNum) / 256;//获取当前列宽度
                for (int rowNum = 0; rowNum <= sheet.LastRowNum; rowNum++)//在这一列上循环行
                {
                    IRow currentRow = sheet.GetRow(rowNum);
                    ICell currentCell = currentRow.GetCell(columnNum);
                    int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;//获取当前单元格的内容宽度
                    if (columnWidth < length)
                    {
                        columnWidth = length;
                    }//若当前单元格内容宽度大于列宽，则调整列宽为当前单元格宽度
                }
                sheet.SetColumnWidth(columnNum, columnWidth * 256);
            }

            //高度自适应 有bug
            /*for (int rowNum = 2; rowNum <= sheet.LastRowNum; rowNum++)
            {
                IRow currentRow = sheet.GetRow(rowNum);
                ICell currentCell = currentRow.GetCell(27);
                int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;
                currentRow.HeightInPoints = 20 * (length / 60 + 1);
            }*/

            MemoryStream stream = new MemoryStream();
            hssfworkbook.Write(stream);

            return stream;
        }
        /// <summary>  
        /// 获取单元格类型(xls)  
        /// </summary>  
        /// <param name="cell"></param>
        /// <param name="isFormulaToNumeric"></param>  
        /// <returns></returns>  
        private object GetValueTypeForXLS(HSSFCell cell, bool isFormulaToNumeric)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                    if (isFormulaToNumeric)
                        return cell.NumericCellValue;
                    else
                        return "=" + cell.CellFormula;
                default:
                    return "=" + cell.CellFormula;
            }
        }
        /// <summary>  
        /// 将Excel文件中的数据读出到DataTable中(xlsx)  
        /// </summary>  
        /// <param name="file">物理路径</param>
        /// <param name="isFormulaToNumeric"></param>  
        /// <returns></returns>  
        public DataTable ExcelToTableForXLSX(string file, bool isFormulaToNumeric = true)
        {
            DataTable dt = new DataTable();
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    dt = ExcelToTableForXLSX(fs, isFormulaToNumeric);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return dt;
        }
        /// <summary>  
        /// 将Excel文件中的数据读出到DataTable中(xlsx)  
        /// </summary>  
        /// <param name="file">文件流</param>
        /// <param name="isFormulaToNumeric"></param>  
        /// <returns></returns>  
        public DataTable ExcelToTableForXLSX(Stream file, bool isFormulaToNumeric = true)
        {
            DataTable dt = new DataTable();
            XSSFWorkbook xssfworkbook = new XSSFWorkbook(file);
            ISheet sheet = xssfworkbook.GetSheetAt(0);

            //表头  
            IRow header = sheet.GetRow(sheet.FirstRowNum);
            List<int> columns = new List<int>();
            for (int i = 0; i < header.LastCellNum; i++)
            {
                object obj = GetValueTypeForXLSX(header.GetCell(i) as XSSFCell, isFormulaToNumeric);
                if (obj == null || obj.ToString() == string.Empty)
                {
                    dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    //continue;  
                }
                else
                    dt.Columns.Add(new DataColumn(obj.ToString()));
                columns.Add(i);
            }
            //数据  
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                DataRow dr = dt.NewRow();
                bool hasValue = false;
                foreach (int j in columns)
                {
                    dr[j] = GetValueTypeForXLSX(sheet.GetRow(i).GetCell(j) as XSSFCell, isFormulaToNumeric);
                    if (dr[j] != null && dr[j].ToString() != string.Empty)
                    {
                        hasValue = true;
                    }
                }
                if (hasValue)
                {
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        /// <summary>  
        /// 将DataTable数据导出到Excel文件中(xlsx)  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <param name="file">物理路径</param>  
        public void TableToExcelForXLSX(DataTable dt, string file)
        {
            MemoryStream stream = (MemoryStream)TableToExcelForXLSX(dt);
            var buf = stream.ToArray();

            //保存为Excel文件
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>  
        /// 将DataTable数据导出到Excel文件中(xlsx)  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <param name="file">物理路径</param>  
        public Stream TableToExcelForXLSX(DataTable dt)
        {
            XSSFWorkbook xssfworkbook = new XSSFWorkbook();
            ISheet sheet = xssfworkbook.CreateSheet("Test");

            //表头  
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //宽度自适应
            if (dt.Columns.Count > 26)
            {
                throw new Exception("列数超出Excel允许的最大值：26");
            }
            for (int columnNum = 0; columnNum < dt.Columns.Count; columnNum++)
            {
                int columnWidth = sheet.GetColumnWidth(columnNum) / 256;//获取当前列宽度
                for (int rowNum = 0; rowNum <= sheet.LastRowNum; rowNum++)//在这一列上循环行
                {
                    IRow currentRow = sheet.GetRow(rowNum);
                    ICell currentCell = currentRow.GetCell(columnNum);
                    int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;//获取当前单元格的内容宽度
                    if (columnWidth < length)
                    {
                        columnWidth = length;
                    }//若当前单元格内容宽度大于列宽，则调整列宽为当前单元格宽度
                }
                sheet.SetColumnWidth(columnNum, columnWidth * 256);
            }

            //高度自适应 有bug
            /*for (int rowNum = 2; rowNum <= sheet.LastRowNum; rowNum++)
            {
                IRow currentRow = sheet.GetRow(rowNum);
                ICell currentCell = currentRow.GetCell(27);
                int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;
                currentRow.HeightInPoints = 20 * (length / 60 + 1);
            }*/

            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            xssfworkbook.Write(stream);

            return stream;
        }
        /// <summary>  
        /// 获取单元格类型(xlsx)  
        /// </summary>  
        /// <param name="cell"></param>
        /// <param name="isFormulaToNumeric"></param>  
        /// <returns></returns>  
        private object GetValueTypeForXLSX(XSSFCell cell, bool isFormulaToNumeric)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                    if (isFormulaToNumeric)
                        return cell.NumericCellValue;
                    else
                        return "=" + cell.CellFormula;
                default:
                    return "=" + cell.CellFormula;
            }
        }
    }
}
