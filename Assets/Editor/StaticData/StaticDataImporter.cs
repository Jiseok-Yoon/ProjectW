using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace ProjectW.Editor
{
    /// <summary>
    /// StaticData 파일이 추가되었을 때 후처리를 진행
    /// excel 파일의 추가를 감지하고, json 파일로 변환한다.
    /// </summary>
    public static class StaticDataImporter
    {
        public static void Import(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            ImportNewOrModified(importedAssets);
            Delete(deletedAssets);
            Move(movedAssets, movedFromAssetPaths);
        }

        /// <summary>
        /// 파일을 삭제한 경우 실행할 기능
        /// </summary>
        /// <param name="deletedAssets"></param>
        private static void Delete(string[] deletedAssets)
        {
            ExcelToJson(deletedAssets, true);
        }

        /// <summary>
        /// 파일이 이동 됐을 때
        /// </summary>
        /// <param name="movedAssets">새로운 경로(이동 후)의 에셋 정보</param>
        /// <param name="movedFromAssetPaths">이전 경로(이동 전)의 에셋 정보</param>
        private static void Move(string[] movedAssets, string[] movedFromAssetPaths)
        {
            // 이전 경로 에셋 삭제 기능 실행
            Delete(movedFromAssetPaths);
            // 새로운 경로 에셋 수정
            ImportNewOrModified(movedAssets);
        }

        /// <summary>
        /// 파일을 새로 임포트하거나 수정했을 때
        /// </summary>
        /// <param name="importedAssets">임포트하거나 수정한 에셋 정보</param>
        private static void ImportNewOrModified(string[] importedAssets)
        {
            ExcelToJson(importedAssets, false);
        }

        private static void ExcelToJson(string[] assets, bool isDeleted)
        {
            List<string> staticDataAssets = new List<string>();

            // 파라미터로 받은 에셋 경로에서 엑셀파일만 걸러낸다.
            foreach (var asset in assets)
            {
                if (IsStaticData(asset, isDeleted))
                    staticDataAssets.Add(asset);
            }

            // 걸러낸 excel 기획데이터를 json으로 변환을 시도한다.
            foreach (var staticDataAsset in staticDataAssets)
            {
                try
                {
                    // 경로에서 파일이름과 확장자만 남긴다.
                    var fileName = staticDataAsset.Substring(staticDataAsset.LastIndexOf('/') + 1);
                    // 확장자를 제거해서 파일이름만 남긴다.
                    fileName = fileName.Remove(fileName.LastIndexOf('.'));

                    var rootPath = Application.dataPath;
                    rootPath = rootPath.Remove(rootPath.LastIndexOf('/'));

                    var fileFullPath = $"{rootPath}/{staticDataAsset}";

                    var excelToJsonConvert = new ExcelToJsonConvert(fileFullPath,
                        $"{rootPath}/{Define.StaticData.SDJsonPath}");

                    // 변환 실행 및 결과를 반환받아 성공했는지 확인
                    if (excelToJsonConvert.SaveJsonFiles() > 0)
                    {
                        AssetDatabase.ImportAsset($"{Define.StaticData.SDJsonPath}/{fileName}.json");
                        Debug.Log($"##### StaticData {fileName} reimported");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogErrorFormat("Couldn't convert assets = {0}", staticDataAsset);
                    EditorUtility.DisplayDialog("Error Convert", 
                        string.Format("Couldn't convert assets = {0}", staticDataAsset),
                        "OK");
                }
            }
        }

        /// <summary>
        /// 파일이 엑셀 파일이면서 기획데이터인지 체크
        /// </summary>
        /// <param name="path">해당 파일 경로</param>
        /// <param name="isDeleted">삭제 이벤트인지?</param>
        /// <returns></returns>
        private static bool IsStaticData(string path, bool isDeleted)
        {
            // excel 파일이 아니라면 리턴
            if (path.EndsWith(".xlsx") == false)
                return false;

            var absolutePath = Application.dataPath + path.Remove(0, "Assets".Length);

            // 삭제이벤트이거나 존재하는 파일어야하고, 경로는 excel데이터 경로에 있어야한다.
            return ((isDeleted || File.Exists(absolutePath)) && path.StartsWith(Define.StaticData.SDExcelPath));
        }
    }
}