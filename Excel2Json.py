import os as os
import pandas as pd
import json
# 获取当前目录
directory_path = os.getcwd()
# 获取Excel的路径
Excel_path = os.path.join(directory_path,'Excel')
print(Excel_path)
# 获取所有Excel名称
entries = os.listdir(Excel_path)
# 获取Json路径
Json_path = os.path.join(directory_path,'Assets','ArtAssets','Jsons')

file_names = []


for entry in entries:   
    if os.path.isfile :
        if not entry.endswith('.py'):
            file_names.append(entry)


for file_name in file_names:
    xls = pd.ExcelFile( os.path.join(Excel_path,file_name))
    for sheet_name in xls.sheet_names:
       df = pd.read_excel(xls,sheet_name=sheet_name)
       data = df.to_dict(orient= 'records')

       sheet_json = json.dumps(data,indent = None,ensure_ascii=False)
       all_sheets_json = {}
       all_sheets_json[sheet_name] = sheet_json
       filename = sheet_name + '.json'
       filename = os.path.join(Json_path,filename)
       with open(filename,'w',encoding="UTF-8") as json_file:
        json.dump(all_sheets_json,json_file,ensure_ascii=False)

       with open(filename,'r',encoding="UTF-8") as json_file1:
           file_content = json_file1.read()
           clean_content = file_content.replace('\\"','"').replace('"[','[').replace(']"',']').replace('NaN','""');

       with open(filename,'w',encoding="UTF-8") as json_file2:
            json_file2.write(clean_content)

       print(filename,'导出成功')
    