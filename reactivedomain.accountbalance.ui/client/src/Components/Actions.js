import React from 'react';
import '../App.css';
class Actions extends React.Component{
    constructor(props){
        super(props);
        this.state={
            action : 'deposit',
            depositType:'',
            wamount:'',
            damount:'',
            olimit: '',
            dlimit : ''
        }
    }
    handleDeposit = (event)=>{
        event.preventDefault();
        if(this.state.damount)
        {
            if(this.state.depositType==="cash"){
            this.props.onCashDeposit(this.state);
        }else if(this.state.depositType==="check"){
            this.props.onCheckDeposit(this.state);
        }
    }
    }
    handleWithdraw = (event)=>{
        event.preventDefault();
        if(this.state.wamount)
            this.props.onWithdraw(this.state);
    }
    handleOLimit =(event)=>{
        event.preventDefault();
        if(this.state.olimit)
        this.props.onSetOverDraftLimit(this.state);
    }
    handleDLimit =(event)=>{
        event.preventDefault();
        if(this.state.dlimit)
        this.props.onSetDailyWireTransferLimit(this.state);
    }
    render(){
        return(
            <ul class="tab-container radio">
            <li>
                <input class="tab-toggle" id="tab-1" type="radio" name="toggle" onClick={()=>this.setState({ action : "deposit"})} checked={this.state.action==="deposit"}/> 
                <label data-title="Tab 1" class="tab l-b" for="tab-1">Deposit</label>            
                <ul class="tab-content-container">
                    <li class="tab-content" >
                        <div style={{marginTop: '10px', marginBottom:'10px'}}><input name="depositType" type="radio" onChange={()=>this.setState({depositType:"cash"})}/>Cash
                        <input name="depositType" type="radio" onChange={()=>this.setState({depositType:"check"})}/>Check
                        </div>
                        <label for="damount">Amount</label>
                        <input type="number" name="damount" onChange={(event)=>this.setState({damount: event.target.value})} value={this.state.damount}/><label>$</label>
                        <div className="outcome">{this.props.msg}</div>
                        <button class="pay-button" onClick={this.handleDeposit}>Deposit</button>
                        <div class="arrows">
                        <label class="back" onClick={()=>this.setState({ action : "daily"})} for="tab-4">&#8249;</label>
                        <label class="next" onClick={()=>this.setState({ action : "withdraw"})} for="tab-2">&#8250;</label>   
                        </div>
                    </li>
                </ul>
            </li>
            <li>
                <input class="tab-toggle" id="tab-2" type="radio" name="toggle" onClick={()=>this.setState({ action : "withdraw"})} checked={this.state.action==="withdraw"}/>
                <label class="tab" for="tab-2">Withdraw</label> 
                <ul class="tab-content-container">
                    <li class="tab-content">
                    <div style={{marginTop: '20px', marginBottom:'20px'}}>
                    <label for="wamount">Amount</label>
                    <input type="number" name="wamount" onChange={(event)=>this.setState({wamount: event.target.value})} value={this.state.wamount}/><label>$</label>
                    </div>
                        <button class="pay-button" onClick={this.handleWithdraw}>Withdraw</button>
                        <div class="arrows">
                        <label class="back" onClick={()=>this.setState({ action : "deposit"})} for="tab-1">&#8249;</label>
                        <label class="next" onClick={()=>this.setState({ action : "overdraft"})} for="tab-3">&#8250;</label>   
                        </div>   
                    </li>
                </ul>
            </li>
            <li>
                <input class="tab-toggle" id="tab-3" type="radio" name="toggle" onClick={()=>this.setState({ action : "overdraft"})} checked={this.state.action==="overdraft"}/>
                <label class="tab" for="tab-3">Overdraft</label> 
                <ul class="tab-content-container">
                    <li class="tab-content">
                    <div style={{marginTop: '20px', marginBottom:'20px'}}>
                    <label for="olimit">Limit</label>
                    <input type="number" name="olimit" onChange={(event)=>this.setState({olimit: event.target.value})} value={this.state.olimit}/><label>$</label>
                    </div>
                        <button class="pay-button" onClick={this.handleOLimit}>Set Overdraft Limit</button>
                        <div class="arrows">
                        <label class="back" onClick={()=>this.setState({ action : "withdraw"})} for="tab-2">&#8249;</label>
                        <label class="next" onClick={()=>this.setState({ action : "daily"})} for="tab-4">&#8250;</label>   
                        </div>   
                    </li>
                </ul>
            </li>
            <li>
                <input class="tab-toggle" id="tab-4" type="radio" name="toggle" onClick={()=>this.setState({ action : "daily"})} checked={this.state.action==="daily"}/>
                <label class="tab" for="tab-4">DailyLimit</label> 
                <ul class="tab-content-container">
                    <li class="tab-content">
                    <div style={{marginTop: '20px', marginBottom:'20px'}}>
                    <label for="dlimit">Limit</label>
                    <input type="number" name="dlimit" onChange={(event)=>this.setState({dlimit: event.target.value})} value={this.state.dlimit}/><label>$</label>
                    </div>
                        <button class="pay-button" onClick={this.handleDLimit}>Set Daily Wire Transfer Limit</button>
                        <div class="arrows">
                        <label class="back" onClick={()=>this.setState({ action : "overdraft"})} for="tab-3">&#8249;</label>
                        <label class="next" onClick={()=>this.setState({ action : "deposit"})} for="tab-1">&#8250;</label>   
                        </div>   
                    </li>
                </ul>
            </li>
            </ul>
            // <div>
            //     <table style={{margin:'0px'}}>
            //         <tr>
            //             <th onClick={()=> this.setState({action:"deposit"})} className={this.state.action=="deposit" ? 'Actions Chosen':'Actions'}>Deposit</th>
            //             <th onClick={()=> this.setState({action:"withdraw"})} className={this.state.action=="withdraw" ? 'Actions Chosen':'Actions'}>Withdraw</th>
            //         </tr>
            //         <tr>
            //             <td colspan='2'> {this.state.action =="deposit" ? <div className="Chosen panel">Saif</div> : null}
            //             {this.state.action == "withdraw" ? <div className="Chosen panel">Ghariani</div> : null}
            //             </td>
            //         </tr>
            //     </table>
            // </div>
        );
    }
}
export default Actions;