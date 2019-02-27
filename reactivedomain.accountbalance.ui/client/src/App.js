import React from 'react';
import Actions from './Components/Actions'
import './App.css';
import SelectAccount from './Components/SelectAccount';
import AccountList from './Components/AccountList';
import dotnetify from 'dotnetify';
import AccountDetails from './Components/AccountDetails';

class App extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
          Id : '',
          holderName:'',
          Message : '',
          CurrentAccount: '',
          AccountsList : []
        }
        this.vm = dotnetify.react.connect('Accounts', this);
        this.dispatch = state => this.vm.$dispatch(state);
        this.dispatchState = state => {
            this.setState(state);
            this.vm.$dispatch(state);
        };
    };
    componentWillUnmount() {
        this.vm.$destroy();
    };

    selectAccount=(selectedAccountInfo) =>{
      this.dispatch({
        Select: {
          Id : selectedAccountInfo.accountId,
          HolderName : selectedAccountInfo.holderName
        }
      });
    };
    changeAccount= ()=>{
      this.dispatch({
        Select: {
          State: 'disconnect'
        }
      });
    };
    addAccount=(Infos)=>{
      this.dispatch({
        Add: Infos.holderName
      });
    };
    depositCheck=(Infos)=>{
      this.dispatch({
        DepositCheck : Infos.damount
      });
    };
    depositCash=(Infos)=>{
      this.dispatch({
        DepositCash : Infos.damount
      });
    };
    withdraw=(Infos)=>{
      this.dispatch({
        Withdraw : Infos.wamount
      })
    }
    setDailyWireTransferLimit=(Infos)=>{
      this.dispatch({
        SetDailyWireTransferLimit : Infos.dlimit
      })
    }
    setOverDraftLimit=(Infos)=>{
      this.dispatch({
        SetOverDraftLimit : Infos.olimit
      })
    }
  render() {
    //console.log(this.state.AccountsList)
    
    return (
      
      <div style={{padding : '20px'}}>
        {this.state.CurrentAccount ? 
        <div>
          <div className='panel'>
          <AccountDetails account={this.state.CurrentAccount} />
          <button onClick={this.changeAccount} className='sign-button'>Change Account</button>
          </div>
          <Actions onCheckDeposit={this.depositCheck} onCashDeposit={this.depositCash} 
          onWithdraw={this.withdraw} onSetDailyWireTransferLimit={this.setDailyWireTransferLimit}
          onSetOverDraftLimit={this.setOverDraftLimit}/>
          <p style={{marginTop:'2px', marginBottom:'2px'}}>{this.state.Message}</p>
        </div> : <div ><div className='panel'>
          <SelectAccount onSubmit={this.selectAccount} onAdd={this.addAccount} message={this.state.Message}/>
          
          </div>
         <AccountList accounts={this.state.AccountsList}/></div>}

      </div>
    );
  }
}

export default App;
