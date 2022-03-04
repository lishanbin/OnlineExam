using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OnlineExam.Model;
using OnlineExam.WebApi.Utility._MD5;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OnlineExam.WebApi.Utility._Npoi
{
    /// <summary>
    /// Excel帮助类
    /// Excel导出操作
    /// 导出Excel文件，这里是通过写入文件流到HttpResponse来完成的，大概流程为：
    /// 1、传递参数
    /// 2、后台通过参数查询符合要求的数据格式
    /// 3、通过数据生成Excel
    /// 4、Excel转换成byte[]
    /// 5、写入文件流HttpResponse
    /// </summary>
    public class ExcelOperationHelper
    {
        /// <summary>
        /// 测试一下如何生成Excel
        /// </summary>
        /// <returns></returns>
        public static IWorkbook CreateExcelWorkbook()
        {
            HSSFWorkbook _Workbook = new HSSFWorkbook();
            ISheet sheet1= _Workbook.CreateSheet("Sheet1");

            //创建第一行
            {
                IRow head = sheet1.CreateRow(0);
                ICell cell = head.CreateCell(0);
                cell.SetCellValue("学生姓名");
                ICell cell1 = head.CreateCell(1);
                cell1.SetCellValue("数学成绩");
                ICell cell2 = head.CreateCell(2);
                cell2.SetCellValue("语文成绩");
            }

            //创建第二行
            {
                IRow head = sheet1.CreateRow(1);
                ICell cell = head.CreateCell(0);
                cell.SetCellValue("张三");
                ICell cell1 = head.CreateCell(1);
                cell1.SetCellValue("100");
                ICell cell2 = head.CreateCell(2);
                cell2.SetCellValue("95");
            }

            return _Workbook;
        }

        /// <summary>
        /// 给定固定格式的数据，可以生成Excel
        /// </summary>
        /// <param name="dataResources"></param>
        /// <returns></returns>
        public static IWorkbook DataToHSSFWorkbook(List<ExcelDataResource> dataResources)
        {
            HSSFWorkbook _Workbook = new HSSFWorkbook();
            if (dataResources==null||dataResources.Count==0)
            {
                return _Workbook;
            }
            //每循环一次，就生成一个Sheet页出来
            foreach (var sheetResource in dataResources)
            {
                if (sheetResource.SheetDataResource==null||sheetResource.SheetDataResource.Count==0)
                {
                    break;
                }
                //创建一个页签
                ISheet sheet = _Workbook.CreateSheet(sheetResource.SheetName);
                //确定当前这一页有多少列---取决保存当前Sheet页数据的实体属性中的标记的特性
                object obj=sheetResource.SheetDataResource[0];

                //获得所有需要导出的列
                Type type=obj.GetType();
                List<PropertyInfo> propertyInfos = type.GetProperties().Where(c => c.IsDefined(typeof(TitleAttribute), true)).ToList();

                //确定表头在哪一行生成
                int titleIndex = 0;
                if (sheetResource.TitleIndex>0)
                {
                    titleIndex = sheetResource.TitleIndex - 1;
                }
                                

                //基于当前的这个Sheet创建表头
                IRow titleRow=sheet.CreateRow(titleIndex);

                ICellStyle style = _Workbook.CreateCellStyle();
                style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Automatic.Index;

                style.Alignment = HorizontalAlignment.CenterSelection;
                style.VerticalAlignment = VerticalAlignment.Center;
                titleRow.Height = 100 * 4;

                for (int i = 0; i < propertyInfos.Count(); i++)
                {
                    TitleAttribute titleAttribute = propertyInfos[i].GetCustomAttribute<TitleAttribute>();
                    ICell cell = titleRow.CreateCell(i);
                    cell.SetCellValue(titleAttribute.Title);
                    cell.CellStyle = style;
                }

                for (int i = 0; i < sheetResource.SheetDataResource.Count(); i++)
                {
                    IRow row = sheet.CreateRow(i + titleIndex+1);
                    object objInstance = sheetResource.SheetDataResource[i];
                    for (int j = 0; j < propertyInfos.Count(); j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(propertyInfos[j].GetValue(objInstance).ToString());
                    }
                }               

            }
            return _Workbook;

        }

        /// <summary>
        /// 传入固定格式的数据，生成Excel Workbook ，再写入到基于内存的流里面去
        /// </summary>
        /// <param name="dataResources"></param>
        /// <returns></returns>
        public static MemoryStream ToExcelMemoryStream(List<ExcelDataResource> dataResources)
        {
            IWorkbook _Workbook=DataToHSSFWorkbook(dataResources);
            using (MemoryStream stream=new MemoryStream())
            {
                _Workbook.Write(stream);
                return stream;
            }
        }

        /// <summary>
        /// 通过数据生成Excel,然后转换成byte[]
        /// </summary>
        /// <param name="dataResources"></param>
        /// <returns></returns>
        public static byte[] ToExcelByteArray(List<ExcelDataResource> dataResources)
        {
            IWorkbook _Workbook = DataToHSSFWorkbook(dataResources);
            using (MemoryStream stream=new MemoryStream())
            {
                _Workbook.Write(stream);
                byte[] bt = stream.ToArray();
                stream.Write(bt, 0, (int)stream.Length);
                return bt;
            }
        }

        /// <summary>
        /// Excel转换成DataTable
        /// </summary>
        /// <param name="hSSFWorkbook"></param>
        /// <returns></returns>
        public static List<DataTable> ToExcelDataTable(IWorkbook hSSFWorkbook)
        {
            List<DataTable> datatableList = new List<DataTable>();
            for (int sheetIndex = 0; sheetIndex < hSSFWorkbook.NumberOfSheets; sheetIndex++)
            {
                ISheet sheet = hSSFWorkbook.GetSheetAt(sheetIndex);
                //获取表头FirstRowNum 第一行索引 0
                IRow header=sheet.GetRow(sheet.FirstRowNum);//获取第一行
                if (header==null)
                {
                    break;
                }
                int startRow = 0; //数据的第一行索引

                DataTable dtNpoi = new DataTable();
                startRow = sheet.FirstRowNum + 1;
                for (int i = header.FirstCellNum; i < header.LastCellNum; i++)
                {
                    ICell cell = header.GetCell(i);
                    if (cell!=null)
                    {
                        string cellValue = $"Column{i + 1}_{cell.ToString()}";
                        if (cellValue!=null)
                        {
                            DataColumn col = new DataColumn(cellValue);
                            dtNpoi.Columns.Add(col);
                        }
                        else
                        {
                            DataColumn col = new DataColumn();
                            dtNpoi.Columns.Add(col);
                        }
                    }
                }

                //数据  LastRowNum 最后一行的索引  如第九行  --- 索引 8
                for (int i = startRow; i < sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i); //获取第i行
                    if (row==null)
                    {
                        continue;
                    }
                    DataRow dr=dtNpoi.NewRow();
                    //遍历每行的单元格
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {
                        if (row.GetCell(j)!=null)
                            dr[j]=row.GetCell(j).ToString();                        
                    }
                    dtNpoi.Rows.Add(dr);
                }
                datatableList.Add(dtNpoi);

            }
            return datatableList;
        }

        /// <summary>
        /// Excel流转化为List<Student>
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static List<Student> ExcelStreamToList(IFormFile file,out string msg)
        {
            msg = "数据处理成功";
            //获取上传文件后缀
            string ext = Path.GetExtension(file.FileName).ToLower();
            if (!ext.Contains("xls")&&!ext.Contains("xlsx"))
            {
                msg = "文件有误，只支持上传XLS、XLSX文件";
                return null;
            }
            //限制单次只能上传5M
            float fileSize = file.Length / 1024 / 1024;
            if (fileSize>5)
            {
                msg = "文件大小超过限制";
                return null;
            }

            try
            {
                //文件处理
                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                //根据Excel版本进行处理
                IWorkbook workbook = ext == ".xls" ? (IWorkbook)new HSSFWorkbook(ms) : new XSSFWorkbook(ms);
                //获取Excel第一张工作表
                ISheet sheet=workbook.GetSheetAt(0);
                //获取数据行数
                int num = sheet.LastRowNum;
                //待处理用户数据
                List<Student> students = new List<Student>();
                for (int i = 1; i <=num; i++)
                {
                    //获取指定行数据
                    IRow row = sheet.GetRow(i);
                    Student student=new Student();
                    student.Username = row.GetCell(1).ToString();
                    student.Password = MD5Helper.MD5Encrypt32("123456");
                    student.Name = row.GetCell(2).ToString();
                    student.Num = int.Parse(row.GetCell(3).ToString());
                    student.State = int.Parse(row.GetCell(4).ToString());
                    student.Role = int.Parse(row.GetCell(5).ToString());
                    student.Adddate = DateTime.Now;
                    students.Add(student);
                }
                return students;
            }
            catch (Exception e)
            {
                msg = "数据处理出错";
            }
            return null;

        }

    }
}
