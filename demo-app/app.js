const $ = id => document.getElementById(id);
let signedIn = sessionStorage.getItem('signedIn') === 'true';
let tasks = JSON.parse(localStorage.getItem('tasks') || '[]');
function render(){
  $('login-view').hidden = signedIn; $('board-view').hidden = !signedIn;
  $('welcome').textContent = 'Welcome, qa_user!';
  const q = $('search').value.trim().toLowerCase();
  const matches = tasks.filter(t => t.title.toLowerCase().includes(q));
  $('task-list').replaceChildren(); $('empty-state').hidden = matches.length > 0;
  for(const task of matches){
    const li=document.createElement('li');li.dataset.testid='task-row';
    const title=document.createElement('span');title.dataset.testid='task-name';title.textContent=task.title;
    const remove=document.createElement('button');remove.textContent='Delete';remove.dataset.testid='delete-task';
    remove.addEventListener('click',()=>{tasks=tasks.filter(t=>t.id!==task.id);save();render();});
    li.append(title,remove);$('task-list').append(li);
  }
}
function save(){localStorage.setItem('tasks',JSON.stringify(tasks));}
$('login-form').addEventListener('submit',e=>{
 e.preventDefault();const user=$('username').value.trim(), pass=$('password').value;
 $('login-error').textContent='';
 if(!user || !pass){$('login-error').textContent='Username and password are required.';return;}
 if(user!=='qa_user' || pass!=='QaDemo123!'){$('login-error').textContent='Invalid username or password.';return;}
 signedIn=true;sessionStorage.setItem('signedIn','true');render();
});
$('logout-button').addEventListener('click',()=>{signedIn=false;sessionStorage.removeItem('signedIn');$('password').value='';render();});
$('task-form').addEventListener('submit',e=>{
 e.preventDefault();$('task-error').textContent='';const title=$('task-title').value.trim();
 if(title.length<1 || title.length>70){$('task-error').textContent='Title must contain 1 to 70 characters.';return;}
 tasks.push({id:crypto.randomUUID(),title});save();$('task-title').value='';render();
});
$('search').addEventListener('input',render);render();
