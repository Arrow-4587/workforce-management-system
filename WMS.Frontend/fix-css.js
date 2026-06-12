const fs = require('fs');
const path = require('path');

function walk(dir, callback) {
  fs.readdirSync(dir).forEach(f => {
    let dirPath = path.join(dir, f);
    let isDirectory = fs.statSync(dirPath).isDirectory();
    isDirectory ? walk(dirPath, callback) : callback(path.join(dir, f));
  });
}

walk('D:\\\\WMS-Solution\\\\WMS.Frontend\\\\src\\\\app', function(filePath) {
  if (filePath.endsWith('.ts')) {
    let content = fs.readFileSync(filePath, 'utf8');
    let original = content;
    
    // Replace hardcoded light backgrounds with CSS variables
    content = content.replace(/background:\s*rgba\(255,\s*255,\s*255,\s*[0-9.]+\)/g, 'background: var(--surface)');
    content = content.replace(/background:\s*#fff/g, 'background: var(--surface)');
    
    // Replace hardcoded dark-transparent backgrounds (often used for hovers or secondary backgrounds in light mode)
    content = content.replace(/background:\s*rgba\(15,\s*23,\s*42,\s*0\.[0-9]+\)/g, 'background: var(--bg)');

    // Fix form input widths so they align properly in grid containers
    content = content.replace(/\.field input, \.field select \{/g, '.field input, .field select { width: 100%;');
    content = content.replace(/\.field input \{/g, '.field input { width: 100%;');
    content = content.replace(/\.field input, \.field textarea \{/g, '.field input, .field textarea { width: 100%;');
    content = content.replace(/\.field select \{/g, '.field select { width: 100%;');

    if (content !== original) {
      fs.writeFileSync(filePath, content, 'utf8');
      console.log('Fixed:', filePath);
    }
  }
});
