using Lib.DTO.Options;
using Lib.DTO.Question;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.File_Utils
{
    public class ExcelReaderUtil
    {
        public class QuestionData
        {
            public string Question { get; set; }
            public string Answer { get; set; }
            public string Correct { get; set; }
        }
        public static List<CreateQuestionDTO> ReadQuestionsFromExcel(byte[] fileBytes)
        {
            var questions = new List<CreateQuestionDTO>();
            CreateQuestionDTO? lastQuestion = null; // To track the last non-empty question

            // Load the Excel file from the byte array
            using (var memoryStream = new MemoryStream(fileBytes))
            {
                using (var package = new ExcelPackage(memoryStream))
                {
                    // Get the first worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    // Assume the data starts from row 2 (row 1 is header)
                    int row = 2;
                    while (worksheet.Cells[row, 1].Value != null || worksheet.Cells[row, 2].Value != null)
                    {
                        string questionText = worksheet.Cells[row, 1].Text;
                        string optionText = worksheet.Cells[row, 2].Text;
                        bool isCorrect =  bool.Parse(worksheet.Cells[row, 3].Text);

                        // If the question text is not empty, create or find the question
                        if (!string.IsNullOrWhiteSpace(questionText))
                        {
                            // Try to find an existing question with the same text
                            lastQuestion = questions.Find(q => q.QuestionText == questionText);

                            // If question doesn't exist, create a new one
                            if (lastQuestion == null)
                            {
                                lastQuestion = new CreateQuestionDTO
                                {
                                    QuestionText = questionText,
                                    Options = new List<CreateOptionDTO>()
                                };
                                questions.Add(lastQuestion);
                            }
                        }

                        // Add the option to the last valid question (even if it's empty)
                        if (lastQuestion != null && !string.IsNullOrWhiteSpace(optionText))
                        {
                            lastQuestion.Options.Add(new CreateOptionDTO
                            {
                                OptionText = optionText,
                                isCorrect = isCorrect
                            });
                        }

                        row++;
                    }
                }
            }

            return questions;
        }




    }

}

