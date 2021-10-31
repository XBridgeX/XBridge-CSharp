using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBridge.Utils
{
    class Cron
    {
        /// <summary>
        /// 解析成Cron表达式
        /// </summary>
        /// <param name="TaskType">任务类型 </param>
        /// <param name="StartYear">开始日期</param>
        /// <param name="EndYear">结束日期</param>
        /// <param name="TaskTime">任务执行时间</param>
        /// <param name="Condition">执行的条件</param>
        /// /// <param name="Spacing">月计划具体执行的日期</param>
        /// <returns></returns>
        public static string ParseCron(TaskType TaskType, string StartDate, string EndDate, string TaskTime, string Condition = "", string Spacing = "")
        {
            if (string.IsNullOrWhiteSpace(TaskTime))
            {
                return string.Empty;
            }
            //1.执行的年份
            string TaskTimeStarYear = ""; //开始年份
            string TaskTimeEndYear = "";  //结束年份
            string TaskTimeYear = "*"; //任务执行的年份
            if (!string.IsNullOrWhiteSpace(StartDate)) TaskTimeStarYear = (DateTime.Parse(StartDate).Year).ToString();
            if (!string.IsNullOrWhiteSpace(EndDate)) TaskTimeEndYear = (DateTime.Parse(EndDate).Year).ToString();
            if (!string.IsNullOrWhiteSpace(TaskTimeStarYear) && !string.IsNullOrWhiteSpace(TaskTimeStarYear)) //如果开始日期和结束日期都不为空
            {
                TaskTimeYear = TaskTimeStarYear + "-" + TaskTimeEndYear;
            }
            else if (string.IsNullOrWhiteSpace(TaskTimeStarYear) && !string.IsNullOrWhiteSpace(TaskTimeStarYear)) //如果只有结束日期，开始日期默认今天
            {
                TaskTimeYear = (DateTime.Now.Year).ToString() + "-" + TaskTimeEndYear;
            }
            else
            {
                TaskTimeYear = "*";
            }
            //1.执行的月份
            string TaskTimeStarMonth = ""; //开始月份
            string TaskTimeEndMonth = "";  //结束月份
            string TaskTimeMonth = "*"; //任务执行的月份
            if (!string.IsNullOrWhiteSpace(StartDate)) TaskTimeStarMonth = (DateTime.Parse(StartDate).Month).ToString();
            if (!string.IsNullOrWhiteSpace(EndDate)) TaskTimeEndMonth = (DateTime.Parse(EndDate).Month).ToString();
            if (!string.IsNullOrWhiteSpace(TaskTimeStarMonth) && !string.IsNullOrWhiteSpace(TaskTimeEndMonth)) //如果开始日期和结束日期都不为空
            {
                TaskTimeMonth = TaskTimeStarMonth + "-" + TaskTimeEndMonth;
            }
            else if (string.IsNullOrWhiteSpace(TaskTimeStarMonth) && !string.IsNullOrWhiteSpace(TaskTimeEndMonth)) //如果只有结束日期，开始日期默认今天
            {
                TaskTimeMonth = (DateTime.Now.Month).ToString() + "-" + TaskTimeEndMonth;
            }
            else
            {
                TaskTimeMonth = "*";
            }

            //2.执行的小时和分钟
            DateTime dtTaskTime = DateTime.Parse(TaskTime);
            string TaskTimeHour = (dtTaskTime.Hour).ToString(); //执行时间的小时
            string TaskTimeMin = (dtTaskTime.Minute).ToString(); //执行时间的分钟
            string result = string.Empty;
            switch (TaskType)
            {
                //日计划任务
                case TaskType.Day:
                    result = string.Format("0 {0} {1} 1/1 {3} ? {2}", TaskTimeMin, TaskTimeHour, TaskTimeYear, TaskTimeMonth);
                    break;
                //周计划 （Condition 已逗号分隔 eg:1,2,5,7） 1：表示星期天  7：表示星期六
                case TaskType.Week:
                    if (string.IsNullOrWhiteSpace(Condition)) Condition = "1-7";
                    result = string.Format("0 {0} {1} ? {4} {2} {3}", TaskTimeMin, TaskTimeHour, Condition, TaskTimeYear, TaskTimeMonth);
                    break;
                //月计划（Condition表示月份 已逗号分隔 eg:1,2,5,7） 1：表示1月  12：表示12月
                case TaskType.Month:
                    if (string.IsNullOrWhiteSpace(Condition)) Condition = "1-12"; //默认1到12月
                    if (string.IsNullOrWhiteSpace(Spacing)) Spacing = "1";   //默认1号发送
                    result = string.Format("0 {0} {1} {2} {3} ? {4}", TaskTimeMin, TaskTimeHour, Spacing, Condition, TaskTimeYear);
                    break;
                default:
                    result = string.Empty;
                    break;
            }
            return result;
        }
    }
    public enum TaskType
    {
        /// <summary>
        /// 每日任务
        /// </summary>
        Day = 0,
        /// <summary>
        /// 每周任务
        /// </summary>
        Week = 1,
        /// <summary>
        /// 每月任务
        /// </summary>
        Month = 2,
        /// <summary>
        /// 一次性任务
        /// </summary>
        Once = 3

    }
}

