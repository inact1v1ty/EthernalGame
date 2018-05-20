using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Util;
using Nethereum.ABI.Encoders;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Signer;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EtheriumAccount {
    private static EtheriumAccount instance = new EtheriumAccount();

    private EtheriumAccount() {}

    public static EtheriumAccount Instance{
        get{
            return instance;
        }
    }

    public string privateKey;
    public string address;
    public string url = "localhost:8545";
    public string addressOwner = "0x6B2Fb2DF3aE3b22B8A4842461231D7B0e556FFD3";

    public void Init(){
        try {
            address = Nethereum.Signer.EthECKey.GetPublicAddress (privateKey);
		} catch (Exception e) {
			Debug.Log("Error importing account from PrivateKey: " + e);
		}
    }

    public IEnumerator getBlockNumber(){
        var blockNumberRequest = new EthBlockNumberUnityRequest(url);
        yield return blockNumberRequest.SendRequest();
        if(blockNumberRequest.Exception == null) {
            var blockNumber = blockNumberRequest.Result.Value;
            Debug.Log("Block: " + blockNumber.ToString());
        }
    }

    public IEnumerator payForCreation(){
        string ABI = @"[{'constant':true,'inputs':[],'name':'ceoAddress','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'creationFee','outputs':[{'name':'','type':'bool'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_newCEO','type':'address'}],'name':'setCEO','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'_adminAddress','type':'address'}],'name':'unassignAdmin','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'gameserverAddress','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_adminAddress','type':'address'}],'name':'assignAdmin','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[],'name':'unpause','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'adminAddresses','outputs':[{'name':'','type':'bool'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'paused','outputs':[{'name':'','type':'bool'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'pause','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'_newGameserver','type':'address'}],'name':'setGameserver','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'ownershipTokenCount','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'uint256'}],'name':'artifactIndexToApproved','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'uint256'}],'name':'artifactIndexToOwner','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'anonymous':false,'inputs':[{'indexed':true,'name':'from','type':'address'},{'indexed':true,'name':'to','type':'address'},{'indexed':true,'name':'tokenId','type':'uint256'}],'name':'Transfer','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'payer','type':'address'}],'name':'PayedCreation','type':'event'},{'constant':false,'inputs':[{'name':'_typeId','type':'uint16'},{'name':'_maxAmount','type':'uint32'},{'name':'_isSellable','type':'bool'},{'name':'_owner','type':'address'}],'name':'createArtifact','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'getArtifactsAmount','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'payForArtifact','outputs':[],'payable':true,'stateMutability':'payable','type':'function'},{'constant':true,'inputs':[{'name':'_id','type':'uint256'}],'name':'getArtifactTypeId','outputs':[{'name':'_typeId','type':'uint16'}],'payable':false,'stateMutability':'view','type':'function'}]";
        string contractAddress = "0xaafa19d6f354eee368e0bc6ed0a418cc8bf49763";
        Contract contract = new Contract(null, ABI, contractAddress);
        var func = contract.GetFunction("payForArtifact");

        // var sha3 = new Nethereum.Util.Sha3Keccack();
        Debug.Log("Owner: "+addressOwner+" , sender: "+address);
        // var hash = sha3.CalculateHashFromHex(address, addressOwner);
        // var signer = new MessageSigner();
        // var signature = signer.Sign (hash.HexToByteArray(), privateKey);
        // var ethEcdsa = MessageSigner.ExtractEcdsaSignature(signature);
        var inp = func.CreateTransactionInput(address, new HexBigInteger(100000), new HexBigInteger(1250000000000000));
        Debug.Log("Generated transaction input");
        var transactionSignedRequest = new TransactionSignedUnityRequest(url, privateKey, address);
        Debug.Log("Sent request");
        yield return transactionSignedRequest.SignAndSendTransaction(inp);

        if (transactionSignedRequest.Exception == null) {
            Debug.Log ("Payed for artifact!");
        }
        else {
            Debug.Log ("Error :(");
        }
    }
}