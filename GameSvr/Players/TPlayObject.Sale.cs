using SystemModule;

namespace GameSvr
{
    
    
    
    public partial class TPlayObject
    {
        
        
        
        
        
        private void UpdateSellOffInfo(int code)
        {
            TDealOffInfo DealOffInfo;
            if (bo_YBDEAL)// 已开通元宝服务
            {
                for (var i = M2Share.sSellOffItemList.Count - 1; i >= 0; i--)
                {
                    if (M2Share.sSellOffItemList.Count <= 0)
                    {
                        break;
                    }
                    DealOffInfo = M2Share.sSellOffItemList[i];
                    if (DealOffInfo != null)
                    {
                        if (DealOffInfo.N == 2)
                        {
                            switch (code)
                            {
                                case 0: 
                                    if (DealOffInfo.sDealCharName == this.m_sCharName)
                                    {
                                        M2Share.sSellOffItemList.RemoveAt(i);
                                        Dispose(DealOffInfo);
                                        break;
                                    }
                                    break;
                                case 1: 
                                    if (DealOffInfo.sBuyCharName == this.m_sCharName)
                                    {
                                        M2Share.sSellOffItemList.RemoveAt(i);
                                        Dispose(DealOffInfo);
                                        break;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        
        
        
        
        
        private void ClientAddSellOffItem(int nItemIdx, string sItemName)
        {
            bool bo11;
            TUserItem UserItem;
            string sUserItemName;
            if (sItemName.IndexOf(' ') >= 0)
            {
                
                HUtil32.GetValidStr3(sItemName, ref sItemName, new char[] { ' ' });
            }
            bo11 = false;
            if (!m_boSellOffOK)
            {
                for (var i = this.m_ItemList.Count - 1; i >= 0; i--)
                {
                    if (this.m_ItemList.Count <= 0)
                    {
                        break;
                    }
                    UserItem = this.m_ItemList[i];
                    if (UserItem == null)
                    {
                        continue;
                    }
                    if (UserItem.MakeIndex == nItemIdx)
                    {
                        
                        sUserItemName = "";
                        if (UserItem.btValue[13] == 1)
                        {
                            sUserItemName = M2Share.ItemUnit.GetCustomItemName(UserItem.MakeIndex, UserItem.wIndex);
                        }
                        if (sUserItemName == "")
                        {
                            sUserItemName = M2Share.UserEngine.GetStdItemName(UserItem.wIndex);
                        }
                        if (string.Compare(sUserItemName, sItemName, StringComparison.OrdinalIgnoreCase) == 0 && m_SellOffItemList.Count < 9)
                        {
                            
                            
                            
                            
                            
                            
                            
                            
                            m_SellOffItemList.Add(UserItem);
                            this.SendMsg(this, Grobal2.RM_SELLOFFADDITEM_OK, 0, 0, 0, 0, ""); 
                            this.m_ItemList.RemoveAt(i);
                            
                            bo11 = true;
                            break;
                        }
                    }
                }
            }
            if (!bo11)
            {
                this.SendMsg(this, Grobal2.RM_SellOffADDITEM_FAIL, 0, 0, 0, 0, "");
            }
        }

        
        
        
        
        
        private void ClientDelSellOffItem(int nItemIdx, string sItemName)
        {
            TUserItem UserItem;
            string sUserItemName = string.Empty;
            if (sItemName.IndexOf(' ') >= 0)
            {
                
                HUtil32.GetValidStr3(sItemName, ref sItemName, new char[] { ' ' });
            }
            bool bo11 = false;
            if (!m_boSellOffOK)
            {
                for (var i = m_SellOffItemList.Count - 1; i >= 0; i--)
                {
                    if (m_SellOffItemList.Count <= 0)
                    {
                        break;
                    }
                    UserItem = m_SellOffItemList[i];
                    if (UserItem == null)
                    {
                        continue;
                    }
                    if (UserItem.MakeIndex == nItemIdx)
                    {
                        if (UserItem.btValue[13] == 1)
                        {
                            sUserItemName = M2Share.ItemUnit.GetCustomItemName(UserItem.MakeIndex, UserItem.wIndex); 
                        }
                        if (string.IsNullOrEmpty(sUserItemName))
                        {
                            sUserItemName = M2Share.UserEngine.GetStdItemName(UserItem.wIndex);
                        }
                        if (string.Compare(sUserItemName, sItemName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            
                            this.m_ItemList.Add(UserItem);
                            this.SendMsg(this, Grobal2.RM_SELLOFFDELITEM_OK, 0, 0, 0, 0, "");
                            m_SellOffItemList.RemoveAt(i);
                            bo11 = true;
                            break;
                        }
                    }
                }
            }
            if (!bo11)
            {
                this.SendMsg(this, Grobal2.RM_SELLOFFDELITEM_FAIL, 0, 0, 0, 0, "");
            }
        }

        
        
        
        private void ClientCancelSellOffIng()
        {
            TDealOffInfo DealOffInfo;
            GoodItem StdItem;
            TUserItem UserItem;
            try
            {
                if (M2Share.sSellOffItemList == null || M2Share.sSellOffItemList.Count == 0 || !IsEnoughBag())
                {
                    return;
                }
                for (var i = M2Share.sSellOffItemList.Count - 1; i >= 0; i--)
                {
                    if (M2Share.sSellOffItemList.Count <= 0)
                    {
                        break;
                    }

                    DealOffInfo = M2Share.sSellOffItemList[i];
                    if (DealOffInfo != null)
                    {
                        if (string.Compare(DealOffInfo.sDealCharName, this.m_sCharName, StringComparison.OrdinalIgnoreCase) == 0 && (DealOffInfo.N == 0 || DealOffInfo.N == 3))
                        {
                            DealOffInfo.N = 4;
                            for (var j = 0; j < 9; j++)
                            {
                                if (DealOffInfo.UseItems[j] == null)
                                {
                                    continue;
                                }
                                StdItem = M2Share.UserEngine.GetStdItem(DealOffInfo.UseItems[j].wIndex);
                                if (StdItem != null)
                                {
                                    
                                    UserItem = DealOffInfo.UseItems[j];
                                    if (IsEnoughBag())// 人物的包裹是否满了
                                    {
                                        if (this.IsAddWeightAvailable(StdItem.Weight))// 检查负重
                                        {
                                            if (this.AddItemToBag(UserItem))
                                            {
                                                SendAddItem(UserItem);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.DropItemDown(UserItem, 3, false, this, this);
                                    }
                                }
                                
                                else if (DealOffInfo.UseItems[j].MakeIndex > 0 && DealOffInfo.UseItems[j].wIndex == short.MaxValue && DealOffInfo.UseItems[j].Dura == short.MaxValue && DealOffInfo.UseItems[j].DuraMax == short.MaxValue)
                                {
                                    m_nGold += DealOffInfo.UseItems[j].MakeIndex; 
                                    this.GameGoldChanged(); 
                                }
                            }
                            M2Share.sSellOffItemList.RemoveAt(i);
                            Dispose(DealOffInfo);
                            this.SendMsg(this, Grobal2.RM_MENU_OK, 0, this.ObjectId, 0, 0, "取消寄售成功!");
                            M2Share.CommonDB.SaveSellOffItemList();//保存元宝寄售列表
                        }
                    }
                }
            }
            catch
            {
                M2Share.MainOutMessage("{异常} TPlayObject.ClientCancelSellOffIng");
            }
        }

        
        
        
        
        private void ClientBuyCancelSellOff(string dealCharName)
        {
            for (var i = M2Share.sSellOffItemList.Count - 1; i >= 0; i--)
            {
                if (M2Share.sSellOffItemList.Count <= 0)
                {
                    break;
                }
                var dealOffInfo = M2Share.sSellOffItemList[i];
                if (dealOffInfo != null)
                {
                    if (string.Compare(dealOffInfo.sDealCharName, dealCharName, StringComparison.OrdinalIgnoreCase) == 0 &&
                        string.Compare(dealOffInfo.sBuyCharName, this.m_sCharName, StringComparison.OrdinalIgnoreCase) == 0 && dealOffInfo.N == 0)
                    {
                        dealOffInfo.N = 3;// 购买人取消标识
                        
                        
                        this.SendMsg(this, Grobal2.RM_MENU_OK, 0, this.ObjectId, 0, 0, "取消交易成功!");
                        break;
                    }
                }
            }
        }

        
        
        
        
        private void ClientBuySellOffItme(string dealCharName)
        {
            GoodItem StdItem;
            TUserItem UserItem;
            TPlayObject PlayObject;
            try
            {
                for (var i = M2Share.sSellOffItemList.Count - 1; i >= 0; i--)
                {
                    if (M2Share.sSellOffItemList.Count <= 0)
                    {
                        break;
                    }
                    var dealOffInfo = M2Share.sSellOffItemList[i];
                    if (dealOffInfo != null)
                    {
                        if (string.Compare(dealOffInfo.sDealCharName, dealCharName, StringComparison.OrdinalIgnoreCase) == 0
                            && string.Compare(dealOffInfo.sBuyCharName, this.m_sCharName, StringComparison.OrdinalIgnoreCase) == 0 && dealOffInfo.N == 0)
                        {
                            dealOffInfo.N = 4;
                            if (m_nGameGold >= dealOffInfo.nSellGold + M2Share.g_Config.nDecUserGameGold)// 每次扣多少元宝(元宝寄售)
                            {
                                m_nGameGold -= dealOffInfo.nSellGold + M2Share.g_Config.nDecUserGameGold; 
                                if (m_nGameGold < 0)
                                {
                                    m_nGameGold = 0;
                                }
                                this.GameGoldChanged(); 
                                PlayObject = M2Share.UserEngine.GetPlayObject(dealOffInfo.sDealCharName);
                                if (PlayObject == null)// 出售人不在线
                                {
                                    dealOffInfo.N = 1; 
                                    
                                    
                                }
                                else
                                {
                                    if (PlayObject.m_boOffLineFlag)  
                                    {
                                        dealOffInfo.N = 1; 
                                    }
                                    else
                                    {
                                        UpdateSellOffInfo(1);
                                        dealOffInfo.N = 2; 
                                        PlayObject.m_nGameGold += dealOffInfo.nSellGold;
                                        PlayObject.GameGoldChanged();
                                        PlayObject.SysMsg(string.Format(M2Share.sGetSellOffGlod, new object[] { dealOffInfo.nSellGold, M2Share.g_Config.sGameGoldName }), MsgColor.Red, MsgType.Hint);
                                        if (M2Share.g_boGameLogGameGold)
                                        {
                                            M2Share.AddGameDataLog(string.Format(M2Share.g_sGameLogMsg1, new object[] { Grobal2.LOG_GAMEGOLD, PlayObject.m_sMapName, PlayObject.m_nCurrX, PlayObject.m_nCurrY, PlayObject.m_sCharName, M2Share.g_Config.sGameGoldName, PlayObject.m_nGameGold, "寄售获得(" + dealOffInfo.nSellGold + ')', this.m_sCharName }));
                                        }
                                    }
                                }
                                M2Share.CommonDB.SaveSellOffItemList();//保存元宝寄售列表
                                for (var j = 0; j <= 9; j++)
                                {
                                    StdItem = M2Share.UserEngine.GetStdItem(dealOffInfo.UseItems[j].wIndex);
                                    if (StdItem != null)
                                    {
                                        
                                        UserItem = dealOffInfo.UseItems[j];
                                        if (IsEnoughBag()) 
                                        {
                                            
                                            if (this.AddItemToBag(UserItem))
                                            {
                                                SendAddItem(UserItem);
                                                if (StdItem.NeedIdentify == 1)
                                                {
                                                    
                                                }
                                            }
                                        }
                                        else
                                        {
                                            this.DropItemDown(UserItem, 3, false, this, this);
                                        }
                                    }
                                    
                                    else if (dealOffInfo.UseItems[j].MakeIndex > 0 && dealOffInfo.UseItems[j].wIndex == short.MaxValue && dealOffInfo.UseItems[j].Dura == short.MaxValue && dealOffInfo.UseItems[j].DuraMax == short.MaxValue)
                                    {
                                        m_nGold += dealOffInfo.UseItems[j].MakeIndex; 
                                        this.SysMsg(dealOffInfo.UseItems[j].MakeIndex + " 颗金刚石增加", MsgColor.Blue, MsgType.Hint);
                                    }
                                }
                                this.SendMsg(this, Grobal2.RM_SELLOFFBUY_OK, 0, 0, 0, 0, "");// 购买成功
                                this.SendMsg(this, Grobal2.RM_MENU_OK, 0, this.ObjectId, 0, 0, "[成功] 系统已经成功接受您的申请");
                                break;
                            }
                            else
                            {
                                dealOffInfo.N = 0;
                                this.SendMsg(this, Grobal2.RM_MENU_OK, 0, this.ObjectId, 0, 0, "[错误] 您的申请提交不成功");
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                M2Share.MainOutMessage("{异常} TPlayObject.ClientBuySellOffItme");
            }
        }

        
        
        
        private void GetSellOffGlod()
        {
            
            
            
            
            try
            {
                for (var i = M2Share.sSellOffItemList.Count - 1; i >= 0; i--)
                {
                    if (M2Share.sSellOffItemList.Count <= 0)
                    {
                        break;
                    }
                    var dealOffInfo = M2Share.sSellOffItemList[i];
                    if (dealOffInfo != null)
                    {
                        if (string.Compare(dealOffInfo.sDealCharName, this.m_sCharName, StringComparison.OrdinalIgnoreCase) == 0 && dealOffInfo.N == 1)
                        {
                            UpdateSellOffInfo(0);
                            dealOffInfo.N = 2; 
                            
                            
                            m_nGameGold += dealOffInfo.nSellGold;
                            this.GameGoldChanged();
                            this.SysMsg(string.Format(M2Share.sGetSellOffGlod, new object[] { dealOffInfo.nSellGold, M2Share.g_Config.sGameGoldName }), MsgColor.Red, MsgType.Hint);
                            if (M2Share.g_boGameLogGameGold)
                            {
                                M2Share.AddGameDataLog(string.Format(M2Share.g_sGameLogMsg1, new object[] { Grobal2.LOG_GAMEGOLD, this.m_sMapName, this.m_nCurrX, this.m_nCurrY,
                                        this.m_sCharName, M2Share.g_Config.sGameGoldName, m_nGameGold, "寄售获得(" + dealOffInfo.nSellGold + ')', dealOffInfo.sBuyCharName }));
                            }
                            break;
                        }
                    }
                }
            }
            catch
            {
                M2Share.MainOutMessage("{异常} TPlayObject.GetSellOffGlod");
            }
        }

        
        
        
        private void ClientCancelSellOff()
        {
            SellOffCancel();
        }

        
        
        
        
        public string SelectSellDate()
        {
            var result = "您未开通" + M2Share.g_Config.sGameGoldName + "寄售服务,请先开通!!!\\ \\<返回/@main>";
            if (bo_YBDEAL)
            {
                
                if (M2Share.sSellOffItemList.Count > 0)
                {
                    for (var i = 0; i < M2Share.sSellOffItemList.Count; i++)
                    {
                        var dealOffInfo = M2Share.sSellOffItemList[i];
                        if (dealOffInfo != null)
                        {
                            if (string.Compare(dealOffInfo.sDealCharName, this.m_sCharName, StringComparison.OrdinalIgnoreCase) == 0 && dealOffInfo.N == 2)
                            {
                                result = "最后一笔出售记录:\\   " + dealOffInfo.dSellDateTime.ToString("yyyy年mm月dd日 hh时ss分") + ",\\  您与" + dealOffInfo.sBuyCharName + "交易成功,获得了" + dealOffInfo.nSellGold + '个' + M2Share.g_Config.sGameGoldName + "。\\ \\<返回/@main>";
                                return result;
                            }
                            else if (string.Compare(dealOffInfo.sBuyCharName, this.m_sCharName, StringComparison.OrdinalIgnoreCase) == 0 && (dealOffInfo.N == 1 || dealOffInfo.N == 2))
                            {
                                result = "最后一笔购买记录:\\   " + dealOffInfo.dSellDateTime.ToString("yyyy年mm月dd日 hh时ss分") + ",\\  您与" + dealOffInfo.sDealCharName + "交易成功,支付了" + dealOffInfo.nSellGold + '个' + M2Share.g_Config.sGameGoldName + "。\\ \\<返回/@main>";
                                return result;
                            }
                        }
                    }
                }
                result = "您未进行任何寄售交易!!!\\ \\<返回/@main>";
            }
            return result;
        }

        
        
        
        
        
        public bool SellOffInTime(int nCode)
        {
            var result = false;
            if (M2Share.sSellOffItemList.Count > 0)
            {
                for (var i = 0; i < M2Share.sSellOffItemList.Count; i++)
                {
                    var dealOffInfo = M2Share.sSellOffItemList[i];
                    if (dealOffInfo != null)
                    {
                        switch (nCode)
                        {
                            case 0: 
                                if (string.Compare(dealOffInfo.sDealCharName, this.m_sCharName, StringComparison.OrdinalIgnoreCase) == 0 && (dealOffInfo.N == 0 || dealOffInfo.N == 3))
                                {
                                    result = true;
                                    break;
                                }
                                break;
                            case 1: 
                                if (string.Compare(dealOffInfo.sBuyCharName, this.m_sCharName, StringComparison.OrdinalIgnoreCase) == 0 && dealOffInfo.N == 0)
                                {
                                    result = true;
                                    break;
                                }
                                break;
                        }
                    }
                }
            }
            return result;
        }

        
        
        
        
        
        
        
        
        private void ClientSellOffEnd(string sBuyCharName, int nSellGold, int nGameDiamond, int nCode)
        {
            TUserItem UserItem;
            GoodItem StdItem;
            TDealOffInfo DealOffInfo;
            m_boSellOffOK = true;
            var bo11 = false;
            if (m_boSellOffOK && (m_SellOffItemList.Count > 0 || nGameDiamond > 0) && m_SellOffItemList.Count < 10 && sBuyCharName.Length < 20 && nSellGold > 0 && nSellGold < 100000000
                && string.Compare(sBuyCharName, this.m_sCharName, StringComparison.OrdinalIgnoreCase) != 0)
            {
                
                DealOffInfo = new TDealOffInfo() { UseItems = new TUserItem[9] };
                if (m_SellOffItemList.Count > 0)
                {
                    for (var i = 0; i < m_SellOffItemList.Count; i++)
                    {
                        UserItem = m_SellOffItemList[i];
                        StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                        if (StdItem != null && UserItem != null && UserItem.MakeIndex > 0)
                        {
                            DealOffInfo.UseItems[i] = UserItem;
                        }
                    }
                }
                for (var j = 0; j < 9; j++)
                {
                    if (DealOffInfo.UseItems[j] == null)
                    {
                        continue;
                    }
                    StdItem = M2Share.UserEngine.GetStdItem(DealOffInfo.UseItems[j].wIndex);
                    if (StdItem == null && nGameDiamond > 0 && nGameDiamond < 10000 && nCode == short.MaxValue)// 物品是金刚石
                    {
                        if (nGameDiamond > m_nGold) 
                        {
                            this.SendMsg(this, Grobal2.RM_SELLOFFEND_FAIL, 0, 0, 0, 0, "");
                            this.SendMsg(this, Grobal2.RM_MENU_OK, 0, this.ObjectId, 0, 0, "[错误] 你没有那么多金币");
                            GetBackSellOffItems(); 
                            return;
                        }
                        m_nGold -= nGameDiamond;
                        this.GameGoldChanged(); 
                        DealOffInfo.UseItems[j].MakeIndex = nGameDiamond; 
                        DealOffInfo.UseItems[j].wIndex = ushort.MaxValue;
                        DealOffInfo.UseItems[j].Dura = ushort.MaxValue;
                        DealOffInfo.UseItems[j].DuraMax = ushort.MaxValue;
                        break;
                    }
                }
                DealOffInfo.sDealCharName = this.m_sCharName; 
                DealOffInfo.sBuyCharName = sBuyCharName.Trim(); 
                DealOffInfo.nSellGold = nSellGold; 
                DealOffInfo.dSellDateTime = DateTime.Now; 
                DealOffInfo.N = 0; 
                M2Share.sSellOffItemList.Add(DealOffInfo); 
                this.SendMsg(this, Grobal2.RM_SELLOFFEND_OK, 0, 0, 0, 0, "");
                m_nGameGold -= M2Share.g_Config.nDecUserGameGold; 
                if (m_nGameGold < 0)
                {
                    m_nGameGold = 0;
                }
                this.SendMsg(this, Grobal2.RM_MENU_OK, 0, this.ObjectId, 0, 0, "[成功] 系统已经成功接受您的申请");
                bo11 = true;
                M2Share.CommonDB.SaveSellOffItemList();//保存元宝寄售列表 
                m_boSellOffOK = false;
                m_SellOffItemList.Clear();
            }
            if (!bo11)
            {
                
                this.SendMsg(this, Grobal2.RM_SELLOFFEND_FAIL, 0, 0, 0, 0, "");
                this.SendMsg(this, Grobal2.RM_MENU_OK, 0, this.ObjectId, 0, 0, "[错误:] 寄售物品失败");
                GetBackSellOffItems();
            }
        }

        
        
        
        private void SellOffCancel()
        {
            this.SendMsg(this, Grobal2.RM_SELLOFFCANCEL, 0, 0, 0, 0, "");
            GetBackSellOffItems();
        }

        
        
        
        public void GetBackSellOffItems()
        {
            if (m_SellOffItemList == null)
            {
                m_SellOffItemList = new List<TUserItem>();
            }
            if (m_SellOffItemList.Count > 0)
            {
                for (var i = m_SellOffItemList.Count - 1; i >= 0; i--)
                {
                    this.m_ItemList.Add(m_SellOffItemList[i]);
                    m_SellOffItemList.RemoveAt(i);
                }
            }
            m_boSellOffOK = false;// 确认元宝寄售标志 
        }
    }
}