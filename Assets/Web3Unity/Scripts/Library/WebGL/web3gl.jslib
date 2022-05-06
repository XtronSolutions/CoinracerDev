mergeInto(LibraryManager.library, {
SetStorage:function(_newkey,_newval){
	//console.log("key: "+Pointer_stringify(_newkey)+" value: "+Pointer_stringify(_newval));
	localStorage.setItem(Pointer_stringify(_newkey), Pointer_stringify(_newval));
  },
  
  GetStorage:function(_newkey2,objectName, callback){
	var parsedObjectName = Pointer_stringify(objectName);
    var parsedCallback = Pointer_stringify(callback);
	var parsedkey = Pointer_stringify(_newkey2);
	
	var _storage=localStorage.getItem(parsedkey);
	
	//console.log("Getkey: "+parsedkey+" value: "+_storage);
	unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(_storage));

  },
  
  GetStorageClass:function(_newkey2,objectName, callback){
	var parsedObjectName = Pointer_stringify(objectName);
    var parsedCallback = Pointer_stringify(callback);
	var parsedkey = Pointer_stringify(_newkey2);
	
	var _storage=localStorage.getItem(parsedkey);
	
	//console.log("Getkey: "+parsedkey+" value: "+_storage);
	
	if(_storage==null)
	{
		unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(_storage));
	}
	else
	{
		unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, _storage);
	}

  },  

Web3Connect: function () {
    window.web3gl.connect();
  },

  ConnectAccount: function () {
    var bufferSize = lengthBytesUTF8(window.web3gl.connectAccount) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(window.web3gl.connectAccount, buffer, bufferSize);
    return buffer;
  },

  SetConnectAccount: function (value) {
    window.web3gl.connectAccount = value;
  },

  SendContractJs: function (method, abi, contract, args, value, gasLimit, gasPrice) {
	var parsedArgs=Pointer_stringify(args);
		var parsedJSOn=JSON.parse(parsedArgs);
		
		if(Pointer_stringify(method)=="endRace")
		{
			var newargs=[parsedJSOn._pid, parsedJSOn._winner,parsedJSOn._score,parsedJSOn._tokenIds,parsedJSOn._hash];
			 window.web3gl.sendContract(
				  Pointer_stringify(method),
				  Pointer_stringify(abi),
				  Pointer_stringify(contract),
				  JSON.stringify(newargs),
				  Pointer_stringify(value),
				  Pointer_stringify(gasLimit),
				  Pointer_stringify(gasPrice)
				);
		}
		else if(Pointer_stringify(method)=="mint")
		{
			var newargs=[parsedJSOn.amount, parsedJSOn.craceValue,parsedJSOn._data];
			 window.web3gl.sendContract(
				  Pointer_stringify(method),
				  Pointer_stringify(abi),
				  Pointer_stringify(contract),
				  JSON.stringify(newargs),
				  Pointer_stringify(value),
				  Pointer_stringify(gasLimit),
				  Pointer_stringify(gasPrice)
				);
		}
		else
		{
  
			window.web3gl.sendContract(
			  Pointer_stringify(method),
			  Pointer_stringify(abi),
			  Pointer_stringify(contract),
			  Pointer_stringify(args),
			  Pointer_stringify(value),
			  Pointer_stringify(gasLimit),
			  Pointer_stringify(gasPrice)
			);
		}
  },

ContractHashJs: function (pid, address, key) {
		var parsedKey=Pointer_stringify(key);
		var parsedAddress=Pointer_stringify(address);
		var parsedpid=Pointer_stringify(pid);


		window.web3gl.ContractHash(parsedpid , parsedAddress, parsedKey);

	},

  SendContractResponse: function () {
    var bufferSize = lengthBytesUTF8(window.web3gl.sendContractResponse) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(window.web3gl.sendContractResponse, buffer, bufferSize);
    return buffer;
  },

  SetContractResponse: function (value) {
    window.web3gl.sendContractResponse = value;
  },


SetEncodedResponse: function (value) {
    window.web3gl.EncodedResponse = value;
  },
  
  SendEncodedResponse: function () {
    var bufferSize = lengthBytesUTF8(window.web3gl.EncodedResponse) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(window.web3gl.EncodedResponse, buffer, bufferSize);
    return buffer;
  },
  
  SendContractEventResponse: function () {
    var bufferSize = lengthBytesUTF8(window.web3gl.sendContractEventResponse) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(window.web3gl.sendContractEventResponse, buffer, bufferSize);
    return buffer;
  },
  
   SetContractEventResponse: function (value) {
    window.web3gl.sendContractEventResponse = value;
  },

  SendTransactionJs: function (to, value, gasLimit, gasPrice) {
    window.web3gl.sendTransaction(
      Pointer_stringify(to),
      Pointer_stringify(value),
      Pointer_stringify(gasLimit),
      Pointer_stringify(gasPrice)
    );
  },

  SendTransactionResponse: function () {
    var bufferSize = lengthBytesUTF8(window.web3gl.sendTransactionResponse) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(window.web3gl.sendTransactionResponse, buffer, bufferSize);
    return buffer;
  },

  SetTransactionResponse: function (value) {
    window.web3gl.sendTransactionResponse = value;
  },

  SignMessage: function (message) {
    window.web3gl.signMessage(Pointer_stringify(message));
  },

  SignMessageResponse: function () {
    var bufferSize = lengthBytesUTF8(window.web3gl.signMessageResponse) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(window.web3gl.signMessageResponse, buffer, bufferSize);
    return buffer; 
  },

  SetSignMessageResponse: function (value) {
    window.web3gl.signMessageResponse = value;
  },

  GetNetwork: function () {
    return window.web3gl.networkId;
  }
});
