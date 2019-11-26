using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using WeChat.Logic.Domain;

namespace WeChat.Logic.BusinessFacade
{
    public class MpUserRelationFO
    {
        WeChatEntities entities = FormulaHelper.GetEntities<WeChatEntities>();

        /// <summary>
        /// 获取人员登录的默认公众号，默认为上次进入的空间，如果用户首次登录，则默认选择第一个用户参与的公众号
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>返回默认的公众号ID</returns>
        public string GetDefaultMpID(string userID)
        {
            string result = string.Empty;
            var t = SysBool.T.ToString();
            var defaultMp = entities.MpAccountUserRelation.Where(d => d.UserID == userID && d.IsDefault == t).OrderByDescending(d => d.ID).FirstOrDefault();
            if (defaultMp != null)
                result = defaultMp.MpID;
            else
            {
                var re = entities.MpAccountUserRelation.FirstOrDefault(d => d.UserID == userID);
                if (re != null)
                {
                    result = re.MpID;
                    this.SetDefaultMp(userID, result);
                }
            }
            return result;
        }

        /// <summary>
        /// 设置用户默认公众号
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="projectInfoID">公众号ID</param>
        public void SetDefaultMp(string userID, string mpID)
        {
            var t = SysBool.T.ToString();
            var f = SysBool.F.ToString();
            var defaultMp = entities.MpAccountUserRelation.Where(d => d.UserID == userID && d.MpID == mpID).OrderByDescending(d => d.ID).FirstOrDefault();
            if (defaultMp != null)
            {
                defaultMp.IsDefault = t;
                defaultMp.IsUsed = t;
            }
            else
            {
                var dm = new MpAccountUserRelation();
                dm.ID = FormulaHelper.CreateGuid();
                dm.UserID = userID;
                dm.MpID = mpID;
                dm.IsDefault = t;
                dm.IsUsed = t;
                entities.MpAccountUserRelation.Add(dm);
            }
            var notdefault = entities.MpAccountUserRelation.Where(d => d.UserID == userID && d.MpID != mpID).ToList();
            for (int i = 0; i < notdefault.Count(); i++)
                notdefault[i].IsDefault = f;

            entities.SaveChanges();
        }

        /// <summary>
        /// 设置常用公众号
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="mpID">公众号ID</param>
        public void SetFocusMp(string userID, string mpID)
        {
            var mprelation = entities.Set<MpAccountUserRelation>().Where(c => c.MpID == mpID && c.UserID == userID).FirstOrDefault();
            if (mprelation == null)
            {
                mprelation = new MpAccountUserRelation();
                mprelation.ID = FormulaHelper.CreateGuid();
                mprelation.MpID = mpID;
                mprelation.UserID = userID;
                mprelation.IsUsed = SysBool.T.ToString();
                entities.Set<MpAccountUserRelation>().Add(mprelation);

            }
            else
            {
                mprelation.IsUsed = SysBool.T.ToString();
            }
            entities.SaveChanges();
        }

        /// <summary>
        /// 取消常用公众号
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="mpID">公众号ID</param>
        public void CancelFocusMp(string userID, string mpID)
        {
            var mprelation = entities.Set<MpAccountUserRelation>().Where(c => c.MpID == mpID && c.UserID == userID).FirstOrDefault();
            if (mprelation != null)
            {
                mprelation.IsUsed = SysBool.F.ToString();
            }
            entities.SaveChanges();
        }
    }
}
